using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;


namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    /// <summary>
    /// Base class for all more or less standard total-score-based competition types where the main difference is in the number of results per competition 
    /// type that are kept.
    /// </summary>
    /// <remarks>
    /// <para>Supports every kind of competition where the arrow scores of a single match are all added up for each participant, after which the 
    /// participants are ranked by class on their total score.</para>
    /// <para>Does, for example, not support final set-scoring where the result of a match is the total of set scores, and the arow scores
    /// are onbly used to calculate the set-score.</para>
    /// </remarks>
    /// <typeparam name="TmatchComparer">A class that contains the logic to compare two TsbParticipantWrapperSingleMatch typed objects to each other to determine which scores best.</typeparam>
    /// <typeparam name="TcompetitionComparer">A class that contains  the logic to compare two TsbParticipantWrapperCompetition typed objects to each other to determine which scores best.</typeparam>
    public abstract class TotalScoreBasedResultCalculatorBase<TmatchComparer, TcompetitionComparer>(IConfiguration configuration)
        : IRuleService
        where TmatchComparer : IComparer<TsbParticipantWrapperSingleMatch>, new()
        where TcompetitionComparer : IComparer<TsbParticipantWrapperCompetition>, new()
    {
        // Point scores for F1 scoring
        private static readonly int[] F1ScoreArray = [12, 10, 8, 7, 6, 5, 4, 3, 2, 1];

        public static readonly GroupInfo KlasseOnbekend = new GroupInfo { Code = "Z#Z", Label = "Klasse onbekend" };

        /// <summary>
        /// By default only the best 4 scores (per Ruleset Code) are used to calculate the total competito result.
        /// </summary>
        protected int RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = 5; // TODO: LOAD FROM MATCH CONFIGURATION IN DB ENTYRY

        /// <summary>
        /// Request supported rulesets to be returned.
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<RulesetModel>> GetSupportedRulesets();

        /// <see cref="IRuleService.CalculateCompetitionResult(int)"></see>
        public virtual async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            using CentaurScoresDbContext db = new(configuration);
            TsbCalculationContext context = new(db, await GetSupportedRulesets());
            var result = await CalculateCompetitionResultForDB(context, competitionId);
            return result;
        }

        /// <see cref="IRuleService.CalculateSingleMatchResult(int)"></see>
        public virtual async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            using CentaurScoresDbContext db = new(configuration);
            TsbCalculationContext context = new(db, await GetSupportedRulesets());
            var result = await CalculateSingleMatchResultForDB(context, matchId, isCompetition: false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> SupportsMatch(MatchEntity matchEntity)
        {
            return (await GetSupportedRulesets()).Any(x => x.Code == matchEntity.RulesetCode && (matchEntity.MatchFlags & MatchEntity.MatchFlagsHeadToHead) == 0); // does not support any final rounds
        }

        /// <inheritdoc/>
        public async Task<bool> SupportsCompetition(CompetitionEntity competitionEntity)
        {
            return (await GetSupportedRulesets()).Any(x => competitionEntity.RulesetGroupName != null && x.GroupName == competitionEntity.RulesetGroupName);
        }

        /// <summary>
        /// Implements all logic required to calculate a competition result for all matches that have been played sofar
        /// as part of that competition. All matches are grouped by ruleset code, and for each ruleset code for each
        /// partitipant the scores that achieved are added up. This gives us a sub-total per ruleset per participant.
        /// 
        /// Next, the sub-totals are added up, yielding a score for the full competition for each participant. These
        /// scores are grouped as requested and returned as a competition result.
        /// </summary>
        protected virtual async Task<CompetitionResultModel> CalculateCompetitionResultForDB(TsbCalculationContext context, int competitionId)
        {
            TsbCompetitionCalculationState state = new()
            {
                DB = context.Database,
                CompetitionEntity = await context.Database.Competitions
                                                .AsNoTracking()
                                                .Include(c => c.Matches)
                                                .ThenInclude(m => m.Participants)
                                                .Include(c => c.ParticipantList)
                                                .ThenInclude(pl => pl!.Entries)
                                                .SingleAsync(c => c.Id == competitionId),
            };

            TsbCompetitionParameters competitionSettings = JsonConvert.DeserializeObject<TsbCompetitionParameters>(state.CompetitionEntity.RulesetParametersJSON ?? "{}") ?? new();
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = competitionSettings.ScoringMatchesAsInt;
            state.UseF1Scoring = competitionSettings.Scoring == "F1";

            // Calculate the "single match result" for each of the matches, then group them by ruleset code
            // sets both score and f1 style points in wrappers
            await PopulateStateSingleMatchResults(context, state);

            // Next, build a list of participants, grouping them by name
            List<ParticipantData> allParticipants = [.. state.MatchResultsByRuleset.SelectMany(mrl => mrl.Value.SelectMany(mr => mr.Ungrouped.Select(p => p.ParticipantData))).Distinct().OrderBy(p => p.Normalizedname)];
            // Create a wrapper, which essentially creates per participant a list of scores (or null) per match.
            List<TsbParticipantWrapperCompetition> wrappers = [];
            foreach (ParticipantData participantData in allParticipants)
            {
                // Taking into account that for undisclosed reasons people may want to switch bow-type
                // halfway through the competition.
                TsbParticipantWrapperCompetition wrapper = new()
                {
                    ParticipantData = participantData,
                };

                // Look up the score for this participant in each of the matches for this competition
                // After this step is run for all participants, all participants have an array per
                // ruleset with per match their score, or NULL if they did not participate in that
                // match. 
                PopulateScoresForEachRulesetAndEachMatchPerRuleset(state, participantData, wrapper, participantData.Group, participantData.Subgroup);

                // If the participant has more than the required number of scores, discard all extra's
                // Discarding is done by setting the discarded boolean in the scoreinfo.
                DiscardExtraScoresForParticipant(state, wrapper);

                // Calculate totals for the non-discarded scores, both per ruleset and ifor the
                // competition
                CalculateTotalScores(state, wrapper);

                // The wrappers now contain all data needed to populate the result. We're going to filter
                // and sort these a couple of times to create our competition results grouped in various
                // ways.
                wrappers.Add(wrapper);
            }

            // Create the result object and populate the basic data
            CompetitionResultModel result = new()
            {
                Name = state.CompetitionEntity.Name,
                RulesetGroupName = state.CompetitionEntity.RulesetGroupName ?? string.Empty,
                RulesetParametersJSON = state.CompetitionEntity.RulesetParametersJSON ?? string.Empty,
                Groups = state.MatchResultsByRuleset.Values.SelectMany(x => x.SelectMany(y => y.Groups)).Distinct().ToList(),
                Subgroups = state.MatchResultsByRuleset.Values.SelectMany(x => x.SelectMany(y => y.Subgroups)).Distinct().ToList(),
            };

            PopulateResultHeadersPerRulesetCode(state, result);

            // Add ungrouped results and sort them
            result.Ungrouped = await SortWrappersUsingCompetitionRules(state, wrappers);

            IEnumerable<string> groupsWithResults = wrappers.Select(x => x.ParticipantData.Group).Distinct();
            foreach (string groupInfo in groupsWithResults)
            {
                // Add results for this group and sort them
                List<TsbParticipantWrapperCompetition> wrappersForThisGroup = wrappers.Where(x => x.ParticipantData.Group == groupInfo).ToList();
                result.ByClass[groupInfo] = await SortWrappersUsingCompetitionRules(state, wrappersForThisGroup);

                result.BySubclass[groupInfo] = [];
                IEnumerable<string> subgroupsWithResults = wrappers.Where(x => x.ParticipantData.Group == groupInfo).Select(x => x.ParticipantData.Subgroup).Distinct().OrderBy(x => x);
                foreach (string subgroupInfo in subgroupsWithResults)
                {
                    // Add results for this group and subgroup and sort them
                    List<TsbParticipantWrapperCompetition> wrappersForThisSubgroup = wrappers.Where(x => x.ParticipantData.Group == groupInfo && x.ParticipantData.Subgroup == subgroupInfo).ToList();
                    result.BySubclass[groupInfo][subgroupInfo] = await SortWrappersUsingCompetitionRules(state, wrappersForThisSubgroup);
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates a match result based on the sum of all scored arrows. Breaks ties by looking at the counts of individual arrow scores, 
        /// starting at the highest possible score and working our way down. If after this the tie remains, both archers will be on the exact
        /// same spot.
        /// </summary>
        protected async Task<MatchResultModel> CalculateSingleMatchResultForDB(TsbCalculationContext context, int id, bool isCompetition)
        {
            MatchEntity? match = await context.Database.GetMatchEntitiesUntracked().Include(x => x.Participants).Include(m => m.Competition).FirstOrDefaultAsync(x => x.Id == id);
            if (null == match)
            {
                throw new ArgumentException("Not a valid match", nameof(id));
            }
            MatchModel matchModel = match.ToModel(); // convert to a model so we can use a utility function to 

            // Populate the list of match participants and add the data that's needed to calculate a result.
            List<TsbParticipantWrapperSingleMatch> participants = await PopulateMatchParticipantsList(context, match, matchModel);

            ParticipantListMetadataModel? listConfig = match.Competition?.ParticipantList?.ToMetadataModel();

            // To the base scores, add data for calculating improvements of the personal best score
            await PopulateAveragesForPersonalBest(context, matchModel, match, participants);

            List<GroupInfo> allClasses = CalculateAllDisciplines(matchModel, listConfig);
            List<DivisionModel> allSubclasses = CalculateAllDivisions(listConfig);

            // Only keep groups and subgroups that are actually used
            List<GroupInfo> disciplineGroupInfos = allClasses.Where(gi => participants.Any(p => p.DisciplineCode == gi.Code)).ToList();
            List<DivisionModel> divisionGroupInfos = allSubclasses.Where(gi => participants.Any(p => p.DivisionGroupCode == gi.Code)).ToList();

            // Create a list of all single arrow scores  that were actually achieved in the match (may be empty, may skip some values)
            List<int> allArrowValues = [.. participants.SelectMany(x => x.Ends.SelectMany(y => y.Arrows.Select(a => a ?? 0))).Distinct().OrderByDescending(x => x)];
            // Pre-populate the tiebreaker dictionaries for all participants
            PrePopulateWrapperAndTiebreakerDictionariesForAllParticipants(matchModel, participants, allArrowValues);

            MatchResultModel result = new()
            {
                Name = match.MatchName,
                Code = match.MatchCode,
                Flags = match.MatchFlags,
                Ruleset = match.RulesetCode ?? string.Empty,
                Groups = allClasses,
                Subgroups = allSubclasses.Select(x => new GroupInfo { Label = x.Label, Code = x.Code }).ToList(),
                Ungrouped = await SortSingleMatchResult(context, allArrowValues, participants, match)
            };
            foreach (GroupInfo disciplineGroupInfo in disciplineGroupInfos)
            {
                var l1participants = participants.Where(x => x.DisciplineCode == disciplineGroupInfo.Code).ToList();
                if (l1participants.Count != 0)
                {
                    result.ByClass[disciplineGroupInfo.Code] = await SortSingleMatchResult(context, allArrowValues, l1participants, match);
                    result.BySubclass[disciplineGroupInfo.Code] = [];
                    foreach (GroupInfo divisionGroupInfo in divisionGroupInfos)
                    {
                        var l2participants = participants.Where(x => x.DisciplineCode == disciplineGroupInfo.Code && x.DivisionGroupCode == divisionGroupInfo.Code).ToList();
                        if (l2participants.Count != 0)
                        {
                            result.BySubclass[disciplineGroupInfo.Code][divisionGroupInfo.Code] = await SortSingleMatchResult(context, allArrowValues, l2participants, match);
                        }
                    }
                }
            }

            return result;
        }

        private static List<DivisionModel> CalculateAllDivisions(ParticipantListMetadataModel? listConfig)
        {
            List<DivisionModel> allSubclasses = listConfig?.Configuration?.Divisions ?? [];
            if (!allSubclasses.Any(g => g.Code == KlasseOnbekend.Code))
            {
                // Ensure we always have the "unknown class" available
                allSubclasses.Add(new DivisionModel { Code = KlasseOnbekend.Code, Label = KlasseOnbekend.Label });
            }

            return allSubclasses;
        }

        private static List<GroupInfo> CalculateAllDisciplines(MatchModel matchModel, ParticipantListMetadataModel? listConfig)
        {
            List<GroupInfo> allClasses = matchModel.Groups;
            foreach (GroupInfo configGroup in listConfig?.Configuration?.Disciplines ?? [])
            {
                if (allClasses.Any(g => g.Code == configGroup.Code))
                {
                    // Consolidate labels with global config
                    GroupInfo existing = allClasses.First(g => g.Code == configGroup.Code);
                    existing.Label = configGroup.Label;
                }
                else if (!allClasses.Any(g => g.Code == configGroup.Code))
                {
                    // Add missing groups from global config
                    allClasses.Add(configGroup);
                }
            }

            return allClasses;
        }

        private async Task PopulateAveragesForPersonalBest(TsbCalculationContext context, MatchModel matchModel, MatchEntity match, List<TsbParticipantWrapperSingleMatch> participants)
        {
            RulesetModel ruleset = context.SupportedRulesets.First(rs => rs.Code == match.RulesetCode);
            string matchCompetitionFormat = ruleset.CompetitionFormat;

            IEnumerable<GroupInfo> groups = matchModel.Groups;

            Dictionary<(int, string), int> personalBest = new();
            List<PersonalBestsListEntryEntity> personalBestEntities = context.Database.PersonalBestListEntries.AsNoTracking().Include(e => e.Participant).AsNoTracking().Where(e => e.List.CompetitionFormat == matchCompetitionFormat).ToList();
            foreach (PersonalBestsListEntryEntity? pbe in personalBestEntities)
            {
                if (pbe.Participant != null && pbe.Participant.Id != null)
                {
                    string disciplineCode = groups.FirstOrDefault(g => g.Label == pbe.Discipline)?.Code ?? string.Empty;
                    personalBest[(pbe.Participant.Id ?? -1, disciplineCode)] = pbe.Score;
                }
            }

            participants.ForEach(p =>
            {
                if (personalBest.TryGetValue((p.Participant.ParticipantListEntryId ?? -2, p.Participant.Group), out int personalBestScore) && match.NumberOfEnds > 0 && match.ArrowsPerEnd > 0)
                {
                    p.Pr = personalBestScore;
                    p.PrPerArrowAverage = (double)personalBestScore / (match.NumberOfEnds * match.ArrowsPerEnd);
                }
                else
                {
                    p.Pr = 0;
                    p.PrPerArrowAverage = 0;
                }
            });

        }

        /// <summary>
        /// For a single category in a match, sort all the results.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="allArrowValues"></param>
        /// <param name="participants"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        protected virtual async Task<List<MatchScoreForParticipant>> SortSingleMatchResult(TsbCalculationContext context, List<int> allArrowValues, List<TsbParticipantWrapperSingleMatch> participants, MatchEntity? match)
        {
            TmatchComparer tiebreakingComparer = new();
            List<MatchScoreForParticipant> result = [];

            participants.ForEach(p =>
            {
                int highestIndex = p.Ends.SelectMany(e => e.Arrows).Select((item, index) => new { item, index }).LastOrDefault((x) => x.item.HasValue)?.index ?? -1;
                if (highestIndex >= 0)
                {
                    p.PerArrowAverage = (double)p.Score / (1 + highestIndex);
                }
                // reset tiebreakers and other temporary data for each list created
                p.TiebreakerArrow = int.MaxValue;
            });

            // The comparer will update p.TiebreakerArrow to be the lowest value on which it needed to compare arrow counts for two records
            // with identical scores, it should be initialized to Int32.MAxValue before each run.
            List<TsbParticipantWrapperSingleMatch> sorted = [.. participants.OrderByDescending(x => x, tiebreakingComparer)];

            for (int index = 0; index < sorted.Count; index++)
            {
                TsbParticipantWrapperSingleMatch pi = sorted[index];
                MatchScoreForParticipant entry = new()
                {
                    ParticipantInfo = $"{pi.Participant.Name}",
                    ParticipantData = pi.ParticipantData,
                    // If consecutive records have the exact same score and tiebreakers won't work, 
                    Position = index + 1,
                    Score = pi.Score,
                    Scores = pi.Scores,
                    PerArrowAverage = pi.PerArrowAverage,
                    PrPerArrowAverage = pi.PrPerArrowAverage,
                    PrScore = pi.Pr,
                    PrAverage = (match?.ArrowsPerEnd ?? 0) * (match?.NumberOfEnds ?? 0) == 0 ? 0.0 : 1.0 * pi.Pr / ((match?.ArrowsPerEnd ?? 1.0) * (match?.NumberOfEnds ?? 1.0)),
                    ScoreInfo = [ new ScoreInfoEntry {
                        IsDiscarded = false,
                        NumberOfArrows = match == null || match.ArrowsPerEnd <= 0 || match.NumberOfEnds <= 0 ? 0 : match.ArrowsPerEnd * match.NumberOfEnds,
                        Score = pi.Score,
                        Scores = pi.Scores,
                        Info = string.Empty
                    }],
                };
                entry.IsPR = entry.PerArrowAverage > pi.PrPerArrowAverage && pi.PrPerArrowAverage > 0.0;

                if (index > 0 && tiebreakingComparer.Compare(sorted[index - 1], sorted[index]) == 0)
                {
                    // if two entries have identical scores and no tiebreaker exists, put them in the same place
                    // in the results and mark them wioth a *
                    result[index - 1].ParticipantInfo = result[index - 1].ParticipantInfo.TrimEnd('*') + "*";
                    entry.Position = result[index - 1].Position;
                    entry.ParticipantInfo += "*";
                }

                if (pi.TiebreakerArrow != int.MaxValue)
                {
                    // If a tiebreak check was needed, add the information used in that check here
                    foreach (int arrow in allArrowValues)
                    {
                        entry.ScoreInfo[0].Info += $"{pi.Tiebreakers[arrow]}x{arrow} ";
                        if (arrow == pi.TiebreakerArrow) break;
                    }
                    entry.ScoreInfo[0].Info = entry.ScoreInfo[0].Info.TrimEnd();
                }

                // Assign a relative point score to the week score and set it both on the entry
                // and in the single element of the scoreinfo.
                entry.ArrowScore = entry.Score;
                entry.ScoreInfo[0].ArrowScore = entry.ScoreInfo[0].Score;
                entry.F1PointScore = F1ScoreArray[Math.Min(entry.Position - 1, F1ScoreArray.Length - 1)];
                entry.ScoreInfo[0].F1PointScore = entry.F1PointScore;

                result.Add(entry);
            }

            return await Task.FromResult(result);
        }

        private static void PrePopulateWrapperAndTiebreakerDictionariesForAllParticipants(MatchModel matchModel, List<TsbParticipantWrapperSingleMatch> participants, List<int> allArrowValues)
        {
            foreach (TsbParticipantWrapperSingleMatch wrapper in participants)
            {
                int ends = wrapper.Ends.Count();
                wrapper.Score = wrapper.Ends.Sum(x => x.Score ?? 0); // sum of all ends
                if (matchModel.ArrowsPerEnd == 3 && matchModel.NumberOfEnds > 10 && matchModel.NumberOfEnds % 10 == 0)
                {
                    CalculateIntermediateResultsPer30Arrows(wrapper, ends);
                }
                else if (matchModel.ArrowsPerEnd == 5 && matchModel.NumberOfEnds > 5 && matchModel.NumberOfEnds % 5 == 0)
                {
                    CalculateIntermediateResultsPer25Arrows(wrapper, ends);
                }
                foreach (int arrow in allArrowValues) wrapper.Tiebreakers[arrow] = 0;
                foreach (var end in wrapper.Ends)
                {
                    foreach (var arrow in end.Arrows)
                    {
                        wrapper.Tiebreakers[arrow ?? 0] += 1;
                    }
                }
            }
        }

        private static void CalculateIntermediateResultsPer30Arrows(TsbParticipantWrapperSingleMatch wrapper, int ends)
        {
            for (int i = 0; i < ends / 10; i++)
            {
                wrapper.Scores.Add(wrapper.Ends.Skip(10 * i).Take(10).Sum(x => x.Score ?? 0));
            }
        }

        private static void CalculateIntermediateResultsPer25Arrows(TsbParticipantWrapperSingleMatch wrapper, int ends)
        {
            for (int i = 0; i < ends / 5; i++)
            {
                wrapper.Scores.Add(wrapper.Ends.Skip(5 * i).Take(5).Sum(x => x.Score ?? 0));
            }
        }

        private async Task<List<TsbParticipantWrapperSingleMatch>> PopulateMatchParticipantsList(TsbCalculationContext context, MatchEntity? match, MatchModel matchModel)
        {
            List<TsbParticipantWrapperSingleMatch> participants = [];

            if (null != match)
            {
                // Adds the (fixed) participant models to the participants array
                participants.AddRange(match.Participants.Select(x =>
                {
                    TsbParticipantWrapperSingleMatch pp = new()
                    {
                        Participant = x.ToSimpleModel(activeRound: 0),
                        ParticipantData = x.ToSimpleModel(activeRound: 0).ToData(),
                        Ends = [],
                        DisciplineCode = x.Group,
                        DivisionGroupCode = string.Empty, // set in fix
                    };
                    OverwriteParticipantInfoFromListData(context, match, x, pp);
                    MatchRepository.AutoFixParticipantModel(matchModel, pp.Participant); // ensures scorecards match the match configuration
                    pp.Ends = pp.Participant.Ends;
                    return pp;
                }));
            }
            return await Task.FromResult(participants);
        }

        private static void OverwriteParticipantInfoFromListData(TsbCalculationContext context, MatchEntity match, ParticipantEntity x, TsbParticipantWrapperSingleMatch pp)
        {
            ParticipantListEntryEntity e = context.GetParticipantEntityFor(x.ParticipantListEntryId ?? -1);
            if (null != e)
            {
                pp.Participant.Name = e.Name;
                pp.ParticipantData.Name = e.Name;
            }

            List<CompetitionFormatDisciplineDivisionMapModel> divisionMappings = context.GetCompetitionFormatDisciplineDivisionMapModel(x.ParticipantListEntryId ?? -1);
            if (null != divisionMappings)
            {
                CompetitionFormatDisciplineDivisionMapModel? mapping = divisionMappings
                        .FirstOrDefault(m => m.CompetitionID == match.Competition?.Id && m.DisciplineCode == pp.Participant.Group);
                if (null != mapping && mapping.DivisionCode != null)
                {
                    pp.Participant.Subgroup = mapping.DivisionCode;
                    pp.ParticipantData.Subgroup = mapping.DivisionCode;
                    pp.DivisionGroupCode = mapping.DivisionCode;
                }
                else
                {
                    pp.Participant.Subgroup = KlasseOnbekend.Code;
                    pp.ParticipantData.Subgroup = KlasseOnbekend.Code;
                    pp.DivisionGroupCode = KlasseOnbekend.Code;
                }
            }
        }

        private async Task PopulateStateSingleMatchResults(TsbCalculationContext context, TsbCompetitionCalculationState state)
        {
            foreach (MatchEntity matchEntity in state.CompetitionEntity.Matches)
            {
                if (null != matchEntity.Id)
                {
                    MatchResultModel matchResult = await CalculateSingleMatchResultForDB(context, matchEntity.Id.Value, isCompetition: true);
                    if (!state.MatchResultsByRuleset.ContainsKey(matchEntity.RulesetCode ?? string.Empty)) state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty] = [];
                    state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty].Add(matchResult);
                }
            }
        }

        private static async Task<List<CompetitionScoreForParticipantModel>> SortWrappersUsingCompetitionRules(
            TsbCompetitionCalculationState state, List<TsbParticipantWrapperCompetition> wrappers)
        {
            var comparer = new TcompetitionComparer();
            var sorted = wrappers.OrderByDescending(x => x, comparer).ToList();

            List<CompetitionScoreForParticipantModel> result = [];

            for (int index = 0; index < sorted.Count; index++)
            {
                var pi = sorted[index];
                List<ScoreInfoEntry> countingMatches = pi.ScoresPerRuleset.SelectMany(kvp => kvp.Value.Where(x => x != null && !x.IsDiscarded)).OfType<ScoreInfoEntry>().ToList();
                int countingArrows = countingMatches.Sum(x => x.NumberOfArrows);
                double countingScore = countingMatches.Sum(x => x.ArrowScore);
                CompetitionScoreForParticipantModel entry = new()
                {
                    ParticipantInfo = $"{pi.ParticipantData.Name}",
                    ParticipantData = pi.ParticipantData,
                    // If consecutive records have the exact same score and tiebreakers won't work, 
                    Position = index + 1,
                    TotalScore = pi.TotalScore,
                    PerArrowAverage = countingArrows == 0 ? 0 : countingScore / countingArrows,
                    PerRuleset = []
                };
                foreach ((string ruleset, List<ScoreInfoEntry?> scores) in pi.ScoresPerRuleset)
                {
                    if (!entry.PerRuleset.ContainsKey(ruleset)) entry.PerRuleset[ruleset] = new CompetitionRulesetResultEntry
                    {
                        TotalScore = pi.TotalScoresPerRuleset[ruleset],
                        Scores = scores
                    };
                }

                if (index > 0 && comparer.Compare(sorted[index - 1], sorted[index]) == 0)
                {
                    // if two entries have identical scores and no tiebreaker exists, put them in the same place
                    // in the results and mark them wioth a *
                    result[index - 1].ParticipantInfo = result[index - 1].ParticipantInfo.TrimEnd('*') + "*";
                    entry.Position = result[index - 1].Position;
                    entry.ParticipantInfo += "*";
                }

                result.Add(entry);
            }

            return await Task.FromResult(result);
        }

        private static void PopulateResultHeadersPerRulesetCode(TsbCompetitionCalculationState state, CompetitionResultModel result)
        {
            foreach ((string ruleset, List<MatchResultModel> matches) in state.MatchResultsByRuleset)
            {
                if (!result.Matches.ContainsKey(ruleset)) result.Matches[ruleset] = new CompetitionResultHeader { Ruleset = ruleset, MatchCodes = [], MatchNames = [] };
                foreach (var match in matches)
                {
                    result.Matches[ruleset].MatchCodes.Add(match.Code);
                    result.Matches[ruleset].MatchNames.Add(match.Name);
                }
            }
        }

        private static void CalculateTotalScores(TsbCompetitionCalculationState state, TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string ruleset, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
                if (state.UseF1Scoring)
                {
                    foreach (ScoreInfoEntry item in scores.OfType<ScoreInfoEntry>())
                    {
                        item.Score = item.F1PointScore;
                    }
                }
                wrapper.TotalScoresPerRuleset[ruleset] = scores.Where(s => s != null && !s.IsDiscarded).Select(s => s?.Score ?? 0).Sum();
            }
            wrapper.TotalScore = wrapper.TotalScoresPerRuleset.Values.Sum();
        }

        private static void PopulateScoresForEachRulesetAndEachMatchPerRuleset(TsbCompetitionCalculationState state, ParticipantData participantData, TsbParticipantWrapperCompetition wrapper, string group, string subGroup)
        {
            foreach ((string rulesetCode, List<MatchResultModel> matchResults) in state.MatchResultsByRuleset)
            {
                foreach (MatchResultModel matchResult in matchResults)
                {
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
                    if (!wrapper.ScoresPerRuleset.ContainsKey(rulesetCode))
                    {
                        wrapper.ScoresPerRuleset[rulesetCode] = [];
                    }

                    if (matchResult.BySubclass.ContainsKey(group) && matchResult.BySubclass[group].ContainsKey(subGroup))
                    {
                        wrapper.ScoresPerRuleset[rulesetCode].Add(matchResult.BySubclass[group][subGroup].FirstOrDefault(p => p.ParticipantData.Equals(participantData))?.ScoreInfo?.FirstOrDefault());
                    }
                    else
                    {
                        wrapper.ScoresPerRuleset[rulesetCode].Add(null); // no score
                    }
#pragma warning restore CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
                }
            }
        }

        private void DiscardExtraScoresForParticipant(TsbCompetitionCalculationState state, TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string rulesetCode, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
                if (state.UseF1Scoring)
                {
                    foreach (var score in scores.OrderByDescending(x => x?.F1PointScore).Skip(RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant))
                    {
                        if (null != score) score.IsDiscarded = true;
                    }
                }
                else
                {
                    foreach (var score in scores.OrderByDescending(x => x?.Score).Skip(RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant))
                    {
                        if (null != score) score.IsDiscarded = true;
                    }
                }
            }
        }
    }
}