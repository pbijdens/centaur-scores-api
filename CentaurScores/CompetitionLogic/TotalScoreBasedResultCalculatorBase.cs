using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;


namespace CentaurScores.CompetitionLogic
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
    public abstract class TotalScoreBasedResultCalculatorBase<TmatchComparer, TcompetitionComparer>
        where TmatchComparer : IComparer<TsbParticipantWrapperSingleMatch>, new()
        where TcompetitionComparer : IComparer<TsbParticipantWrapperCompetition>, new()
    {
        /// <summary>
        /// By default only the best 4 scores (per Ruleset Code) are used to calculate the total competito result.
        /// </summary>
        protected int RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = 4; // TODO: LOAD FROM MATCH CONFIGURATION IN DB ENTYRY

        /// <summary>
        /// Request suppoerted rulesets to be returned.
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<RulesetModel>> GetSupportedRulesets();

        /// <summary>
        /// Implements all logic required to calculate a competition result for all matches that have been played sofar
        /// as part of that competition. All matches are grouped by ruleset code, and for each ruleset code for each
        /// partitipant the scores that achieved are added up. This gives us a sub-total per ruleset per participant.
        /// 
        /// Next, the sub-totals are added up, yielding a score for the full competition for each participant. These
        /// scores are grouped as requested and returned as a competition result.
        /// </summary>
        protected virtual async Task<CompetitionResultModel> CalculateCompetitionResultForDB(CentaurScoresDbContext db, int competitionId)
        {
            TsbCompetitionCalculationState state = new()
            {
                DB = db,
                CompetitionEntity = await db.Competitions
                                                .AsNoTracking()
                                                .Include(c => c.Matches)
                                                .ThenInclude(m => m.Participants)
                                                .Include(c => c.ParticipantList)
                                                .ThenInclude(pl => pl!.Entries)
                                                .SingleAsync(c => c.Id == competitionId)
            };

            // Calculate the "single match result" for each of the matches, then group them by ruleset code
            await PopulateStateSingleMatchResults(db, state);

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
                DiscardExtraScoresForParticipant(wrapper);

                // Calculate totals for the non-discarded scores, both per ruleset and ifor the
                // competition
                CalculateTotalScores(wrapper);

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
            result.Ungrouped = await SortWrappersUsingCompetitionRules(wrappers);

            IEnumerable<string> groupsWithResults = wrappers.Select(x => x.ParticipantData.Group).Distinct();
            foreach (string groupInfo in groupsWithResults)            
            {
                // Add results for this group and sort them
                List<TsbParticipantWrapperCompetition> wrappersForThisGroup = wrappers.Where(x => x.ParticipantData.Group == groupInfo).ToList();
                result.ByClass[groupInfo] = await SortWrappersUsingCompetitionRules(wrappersForThisGroup);

                result.BySubclass[groupInfo] = [];
                IEnumerable<string> subgroupsWithResults = wrappers.Where(x => x.ParticipantData.Group == groupInfo).Select(x => x.ParticipantData.Subgroup).Distinct();
                foreach (string subgroupInfo in subgroupsWithResults)
                {
                    // Add results for this group and subgroup and sort them
                    List<TsbParticipantWrapperCompetition> wrappersForThisSubgroup = wrappers.Where(x => x.ParticipantData.Group == groupInfo && x.ParticipantData.Subgroup == subgroupInfo).ToList();
                    result.BySubclass[groupInfo][subgroupInfo] = await SortWrappersUsingCompetitionRules(wrappersForThisSubgroup);
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates a match result based on the sum of all scored arrows. Breaks ties by looking at the counts of individual arrow scores, 
        /// starting at the highest possible score and working our way down. If after this the tie remains, both archers will be on the exact
        /// same spot.
        /// </summary>
        protected virtual async Task<MatchResultModel> CalculateSingleMatchResultForDB(CentaurScoresDbContext db, int id)
        {
            MatchEntity? match = await db.Matches.AsNoTracking().Include(x => x.Participants).Include(x => x.Competition).FirstOrDefaultAsync(x => x.Id == id);
            if (null == match)
            {
                throw new ArgumentException("Not a valid match", nameof(id));
            }
            MatchModel matchModel = match.ToModel(); // convert to a model so we can use a utility function to 

            // Populate the list of match participants and add the data that's needed to calculate a result.
            List<TsbParticipantWrapperSingleMatch> participants = PopulateMatchParticipantsList(match, matchModel);

            // To the base scores, add data for calculating improvements of the personal best score
            await PopulateAveragesForPersonalBest(db, match, participants);

            // Only process groups and subgroups that are actually used
            List<GroupInfo> allClasses = JsonConvert.DeserializeObject<List<GroupInfo>>(match.GroupsJSON) ?? [];
            List<GroupInfo> allSubclasses = JsonConvert.DeserializeObject<List<GroupInfo>>(match.SubgroupsJSON) ?? [];
            List<GroupInfo> disciplineGroupInfos = allClasses.Where(gi => participants.Any(p => p.DisciplineCode == gi.Code)).ToList();
            List<GroupInfo> ageGroupGroupInfos = allSubclasses.Where(gi => participants.Any(p => p.AgeGroupCode == gi.Code)).ToList();
            // Create a list of all single arrow scores  that were actually achieved in the match (may be empty, may skip some values)
            List<int> allArrowValues = [.. participants.SelectMany(x => x.Ends.SelectMany(y => y.Arrows.Select(a => a ?? 0))).Distinct().OrderByDescending(x => x)];
            // Pre-populate the tiebreaker dictionaries for all participants
            PrePopulateTiebreakerDictionariesForAllParticipants(participants, allArrowValues);

            MatchResultModel result = new()
            {
                Name = match.MatchName,
                Code = match.MatchCode,
                Ruleset = match.RulesetCode ?? string.Empty,
                Groups = allClasses,
                Subgroups = allSubclasses,
                Ungrouped = await SortSingleMatchResult(db, allArrowValues, participants, match)
            };
            foreach (GroupInfo disciplineGroupInfo in disciplineGroupInfos)
            {
                var l1participants = participants.Where(x => x.DisciplineCode == disciplineGroupInfo.Code).ToList();
                if (l1participants.Count != 0)
                {
                    result.ByClass[disciplineGroupInfo.Code] = await SortSingleMatchResult(db, allArrowValues, l1participants, match);
                    result.BySubclass[disciplineGroupInfo.Code] = [];
                    foreach (GroupInfo ageGroupGroupInfo in ageGroupGroupInfos)
                    {
                        var l2participants = participants.Where(x => x.DisciplineCode == disciplineGroupInfo.Code && x.AgeGroupCode == ageGroupGroupInfo.Code).ToList();
                        if (l2participants.Count != 0)
                        {
                            result.BySubclass[disciplineGroupInfo.Code][ageGroupGroupInfo.Code] = await SortSingleMatchResult(db, allArrowValues, l2participants, match);
                        }
                    }
                }
            }

            return result;
        }

        private async Task PopulateAveragesForPersonalBest(CentaurScoresDbContext db, MatchEntity match, List<TsbParticipantWrapperSingleMatch> participants)
        {
            RulesetModel ruleset = (await GetSupportedRulesets()).First(rs => rs.Code == match.RulesetCode);
            string matchCompetitionFormat = ruleset.CompetitionFormat;

            GroupInfo[] groups = JsonConvert.DeserializeObject<GroupInfo[]>(match.GroupsJSON) ?? [];

            Dictionary<(int,string), int> personalBest = new();
            List<PersonalBestsListEntryEntity> personalBestEntities = db.PersonalBestListEntries.Include(e => e.Participant).AsNoTracking().Where(e => e.List.CompetitionFormat == matchCompetitionFormat).ToList();
            foreach (PersonalBestsListEntryEntity? pbe in personalBestEntities)
            {
                if (pbe.Participant != null && pbe.Participant.Id != null)
                {
                    string disciplineCode = groups.FirstOrDefault(g => g.Label == pbe.Discipline)?.Code ?? string.Empty;
                    personalBest[(pbe.Participant.Id ?? -1, disciplineCode)] = pbe.Score;
                }
            }

            participants.ForEach(p => { 
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
        protected virtual async Task<List<MatchScoreForParticipant>> SortSingleMatchResult(CentaurScoresDbContext db, List<int> allArrowValues, List<TsbParticipantWrapperSingleMatch> participants, MatchEntity? match)
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
                    PerArrowAverage = pi.PerArrowAverage,
                    PrPerArrowAverage = pi.PrPerArrowAverage,
                    PrScore = pi.Pr,
                    PrAverage = ((match?.ArrowsPerEnd ?? 0) * (match?.NumberOfEnds ?? 0)) == 0 ? 0.0 : (1.0 * pi.Pr / ((match?.ArrowsPerEnd ?? 1.0) * (match?.NumberOfEnds ?? 1.0))),
                    ScoreInfo = [ new ScoreInfoEntry {
                        IsDiscarded = false,
                        NumberOfArrows = (match == null || match.ArrowsPerEnd <= 0 || match.NumberOfEnds <= 0) ? 0 : (match.ArrowsPerEnd * match.NumberOfEnds),
                        Score = pi.Score,
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

                result.Add(entry);
            }

            return await Task.FromResult(result);
        }

        private static void PrePopulateTiebreakerDictionariesForAllParticipants(List<TsbParticipantWrapperSingleMatch> participants, List<int> allArrowValues)
        {
            foreach (TsbParticipantWrapperSingleMatch wrapper in participants)
            {
                wrapper.Score = wrapper.Ends.Sum(x => x.Score ?? 0); // sum of all ends
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

        private static List<TsbParticipantWrapperSingleMatch> PopulateMatchParticipantsList(MatchEntity? match, MatchModel matchModel)
        {
            List<TsbParticipantWrapperSingleMatch> participants = [];

            if (null != match)
            {
                // Adds the (fixed) participant models to the participants array
                participants.AddRange(match.Participants.Select(x =>
                {
                    TsbParticipantWrapperSingleMatch pp = new()
                    {
                        Participant = x.ToModel(),
                        ParticipantData = x.ToModel().ToData(),
                        Ends = [],
                        DisciplineCode = x.Group,
                        AgeGroupCode = x.Subgroup,
                    };
                    MatchRepository.AutoFixParticipantModel(matchModel, pp.Participant); // ensures scorecards match the match configuration
                    pp.Ends = pp.Participant.Ends;
                    return pp;
                }));
            }
            return participants;
        }

        private async Task PopulateStateSingleMatchResults(CentaurScoresDbContext db, TsbCompetitionCalculationState state)
        {
            foreach (MatchEntity matchEntity in state.CompetitionEntity.Matches)
            {
                if (null != matchEntity.Id)
                {
                    MatchResultModel matchResult = await CalculateSingleMatchResultForDB(db, matchEntity.Id.Value);
                    if (!state.MatchResultsByRuleset.ContainsKey(matchEntity.RulesetCode ?? string.Empty)) state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty] = [];
                    state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty].Add(matchResult);
                }
            }
        }

        private static async Task<List<CompetitionScoreForParticipantModel>> SortWrappersUsingCompetitionRules(List<TsbParticipantWrapperCompetition> wrappers)
        {
            var comparer = new TcompetitionComparer();
            var sorted = wrappers.OrderByDescending(x => x, comparer).ToList();

            List<CompetitionScoreForParticipantModel> result = [];

            for (int index = 0; index < sorted.Count; index++)
            {
                var pi = sorted[index];
                List<ScoreInfoEntry> countingMatches = pi.ScoresPerRuleset.SelectMany(kvp => kvp.Value.Where(x => x != null && !x.IsDiscarded)).OfType<ScoreInfoEntry>().ToList();
                int countingArrows = countingMatches.Sum(x => x.NumberOfArrows);
                double countingScore = countingMatches.Sum(x => x.Score);
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

        private static void CalculateTotalScores(TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string ruleset, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
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

        private void DiscardExtraScoresForParticipant(TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string rulesetCode, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
                foreach (var score in scores.OrderByDescending(x => x?.Score).Skip(RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant))
                {
                    if (null != score) score.IsDiscarded = true;
                }
            }
        }
    }
}