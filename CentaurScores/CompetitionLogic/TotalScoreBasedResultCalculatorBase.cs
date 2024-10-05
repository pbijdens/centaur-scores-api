using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;

namespace CentaurScores.CompetitionLogic
{
    public class TotalScoreBasedResultCalculatorBase<Tcomparer, TcompetitionComparar>
        where Tcomparer : IComparer<TsbParticipantWrapperSingleMatch>, new()
        where TcompetitionComparar : IComparer<TsbParticipantWrapperCompetition>, new()
    {
        private const int RequiredNumberOfMatchesSHouldBePartOfTheCompetitionEntity = 4;

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

            // Consolidate participants, make sure they are all linked up properly
            await MatchHelpers.ConsolidateParticipantsForCompetition(state);

            // Now, calculate a single match result for each of the matches and group them by ruleset
            foreach (MatchEntity matchEntity in state.CompetitionEntity.Matches)
            {
                if (null != matchEntity.Id)
                {
                    MatchResultModel matchResult = await CalculateSingleMatchResultForDB(db, matchEntity.Id.Value);
                    if (!state.MatchResultsByRuleset.ContainsKey(matchEntity.RulesetCode ?? string.Empty)) state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty] = [];
                    state.MatchResultsByRuleset[matchEntity.RulesetCode ?? string.Empty].Add(matchResult);
                }
            }

            // Next, build a list of participants, grouping them by name
            List<string> allParticipants = state.MatchResultsByRuleset.SelectMany(mrl => mrl.Value.SelectMany(mr => mr.Ungrouped.Select(p => $"{p.ParticipantInfo}".TrimEnd('*')))).OrderBy(x => x).Distinct().ToList();

            // Create a wrapper, which essentially creates per participant a list of scores (or null) per match.
            List<TsbParticipantWrapperCompetition> wrappers = [];
            foreach (string participantName in allParticipants)
            {
                foreach ((string group, string subgroup) in GroupsForParticipant(db, state, participantName))
                {
                    // Taking into account that for undisclosed reasons people may want to switch bow-type
                    // halfway through the competition.
                    TsbParticipantWrapperCompetition wrapper = new()
                    {
                        ParticipantName = participantName,
                        Group = group,
                        Subgroup = subgroup,
                    };

                    // Look up the score for this participant in each of the matches for this competition
                    // After this step is run for all participants, all participants have an array per
                    // ruleset with per match their score, or NULL if they did not participate in that
                    // match. 
                    PopulateScoresForEachRulesetAndEachMatchPerRuleset(state, participantName, wrapper, group, subgroup);

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
            }

            // sort the wrappers
            // create resultset
            CompetitionResultModel result = new CompetitionResultModel
            {
                Name = state.CompetitionEntity.Name,
                RulesetGroupName = state.CompetitionEntity.RulesetGroupName ?? string.Empty,
                Groups = state.MatchResultsByRuleset.Values.SelectMany(x => x.SelectMany(y => y.Groups)).Distinct().ToList(),
                Subgroups = state.MatchResultsByRuleset.Values.SelectMany(x => x.SelectMany(y => y.Subgroups)).Distinct().ToList(),
            };

            PopulateResultHeadersPerRulesetCode(state, result);

            result.Ungrouped = await SortCompetitionResult(db, wrappers);
            foreach (string classGroupInfo in wrappers.Select(x => x.Group).Distinct())
            {
                result.ByClass[classGroupInfo] = await SortCompetitionResult(db, wrappers.Where(x => x.Group == classGroupInfo).ToList());
                result.BySubclass[classGroupInfo] = new Dictionary<string, List<CompetitionResultEntry>>();
                foreach (string subclassGroupInfo in wrappers.Where(x => x.Group == classGroupInfo).Select(x => x.Subgroup).Distinct())
                {
                    result.BySubclass[classGroupInfo][subclassGroupInfo] = await SortCompetitionResult(db, wrappers.Where(x => x.Group == classGroupInfo && x.Subgroup == subclassGroupInfo).ToList());
                }
            }

            return result;
        }

        private List<(string group, string subgroup)> GroupsForParticipant(CentaurScoresDbContext db, TsbCompetitionCalculationState state, string name)
        {
            return state.CompetitionEntity.Matches.SelectMany(m => m.Participants.Where(p => p.Name == name)).ToList().Select(x => (group: x.Group, subgroup: x.Subgroup)).Distinct().ToList();
        }

        private async Task<List<CompetitionResultEntry>> SortCompetitionResult(CentaurScoresDbContext db, List<TsbParticipantWrapperCompetition> wrappers)
        {
            var comparer = new TcompetitionComparar();
            var sorted = wrappers.OrderByDescending(x => x, comparer).ToList();

            List<CompetitionResultEntry> result = [];

            for (int index = 0; index < sorted.Count; index++)
            {
                var pi = sorted[index];
                CompetitionResultEntry entry = new()
                {
                    ParticipantInfo = $"{pi.ParticipantName}",
                    // If consecutive records have the exact same score and tiebreakers won't work, 
                    Position = index + 1,
                    TotalScore = pi.TotalScore,
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

        private void CalculateTotalScores(TsbCompetitionCalculationState state, TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string ruleset, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
                wrapper.TotalScoresPerRuleset[ruleset] = scores.Where(s => s != null && !s.IsDiscarded).Select(s => s?.Score ?? 0).Sum();
            }
            wrapper.TotalScore = wrapper.TotalScoresPerRuleset.Values.Sum();
        }

        private static void PopulateScoresForEachRulesetAndEachMatchPerRuleset(TsbCompetitionCalculationState state, string participantName, TsbParticipantWrapperCompetition wrapper, string group, string subGroup)
        {
            foreach ((string rulesetCode, List<MatchResultModel> matchResults) in state.MatchResultsByRuleset)
            {
                foreach (MatchResultModel matchResult in matchResults)
                {
                    if (!wrapper.ScoresPerRuleset.ContainsKey(rulesetCode)) wrapper.ScoresPerRuleset[rulesetCode] = [];
                    if (matchResult.BySubclass.ContainsKey(group) && matchResult.BySubclass[group].ContainsKey(subGroup))
                    {
                        wrapper.ScoresPerRuleset[rulesetCode].Add(matchResult.BySubclass[group][subGroup].FirstOrDefault(p => $"{p.ParticipantInfo}".TrimEnd('*') == participantName)?.ScoreInfo?.FirstOrDefault());
                    } else
                    {
                        wrapper.ScoresPerRuleset[rulesetCode].Add(null); // no score
                    }
                }
            }
        }

        private static void DiscardExtraScoresForParticipant(TsbCompetitionCalculationState state, TsbParticipantWrapperCompetition wrapper)
        {
            foreach ((string rulesetCode, List<ScoreInfoEntry?> scores) in wrapper.ScoresPerRuleset)
            {
                foreach (var score in scores.OrderByDescending(x => x?.Score).Skip(RequiredNumberOfMatchesSHouldBePartOfTheCompetitionEntity))
                {
                    if (null != score) score.IsDiscarded = true;
                }
            }
        }

        /// <summary>
        /// Calculates a matych result based on the sum of all scored arrows. Breaks ties by looking at the counts of individual arrow scores, 
        /// starting at the highest possible score and working our way down. If after this the tie remains, both archers will be on the exact
        /// same spot.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="matchId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected virtual async Task<MatchResultModel> CalculateSingleMatchResultForDB(CentaurScoresDbContext db, int matchId)
        {
            MatchEntity? match = await db.Matches.AsNoTracking().Include(x => x.Participants).Include(x => x.Competition).FirstOrDefaultAsync(x => x.Id == matchId);
            if (null == match)
            {
                throw new ArgumentException(nameof(matchId), "Not an identifier");
            }
            MatchModel matchModel = match.ToModel(); // convert to a model so we can use a utility function to 
            List<GroupInfo> allClasses = JsonConvert.DeserializeObject<List<GroupInfo>>(match.GroupsJSON) ?? [];
            List<GroupInfo> allSubclasses = JsonConvert.DeserializeObject<List<GroupInfo>>(match.SubgroupsJSON) ?? [];

            List<TsbParticipantWrapperSingleMatch> participants = [];

            // Adds the (fixed) participant models to the participants array
            participants.AddRange(match.Participants.Select(x =>
            {
                TsbParticipantWrapperSingleMatch pp = new()
                {
                    Participant = x.ToModel(),
                    Ends = [],
                    ClassCode = x.Group,
                    SubclassCode = x.Subgroup
                };
                MatchRepository.AutoFixParticipantModel(matchModel, pp.Participant); // ensures scorecards match the match configuration
                pp.Ends = pp.Participant.Ends;
                return pp;
            }));

            // Only process classes and subclasses that are actually used
            List<GroupInfo> classes = allClasses.Where(gi => participants.Any(p => p.ClassCode == gi.Code)).ToList();
            List<GroupInfo> subclasses = allSubclasses.Where(gi => participants.Any(p => p.SubclassCode == gi.Code)).ToList();

            // Create a list of all single arrow scores  that were actually achieved in the match (may be empty)
            List<int> allArrowValues = participants.SelectMany(x => x.Ends.SelectMany(y => y.Arrows.Select(a => a ?? 0))).Distinct().OrderByDescending(x => x).ToList();

            // Create an array of wrapper-objects that we use for keeping track of temp data during the result calculation
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

            MatchResultModel result = new();

            result.Name = match.MatchName;
            result.Code = match.MatchCode;
            result.Ruleset = match.RulesetCode ?? string.Empty;
            result.Groups = allClasses;
            result.Subgroups = allSubclasses;

            result.Ungrouped = await SortSingleMatchResult(db, allArrowValues, participants);
            foreach (GroupInfo classGroupInfo in classes)
            {
                result.ByClass[classGroupInfo.Code] = await SortSingleMatchResult(db, allArrowValues, participants.Where(x => x.ClassCode == classGroupInfo.Code).ToList());
                result.BySubclass[classGroupInfo.Code] = new Dictionary<string, List<MatchResultEntry>>();
                foreach (GroupInfo subclassGroupInfo in subclasses)
                {
                    result.BySubclass[classGroupInfo.Code][subclassGroupInfo.Code] = await SortSingleMatchResult(db, allArrowValues, participants.Where(x => x.ClassCode == classGroupInfo.Code && x.SubclassCode == subclassGroupInfo.Code).ToList());
                }
            }

            return result;
        }

        protected virtual async Task<List<MatchResultEntry>> SortSingleMatchResult(CentaurScoresDbContext db, List<int> allArrowValues, List<TsbParticipantWrapperSingleMatch> participants)
        {
            Tcomparer tiebreakingComparer = new();
            List<MatchResultEntry> result = new();

            participants.ForEach(p =>
            {
                // reset tiebreakers and other temporary data for each list created
                p.TiebreakerArrow = int.MaxValue;
            });

            // The comparer will update p.TiebreakerArrow to be the lowest value on which it needed to compare arrow counts for two records
            // with identical scores, it should be initialized to Int32.MAxValue before each run.
            List<TsbParticipantWrapperSingleMatch> sorted = participants.OrderByDescending(x => x, tiebreakingComparer).ToList();

            for (int index = 0; index < sorted.Count; index++)
            {
                var pi = sorted[index];
                MatchResultEntry entry = new()
                {
                    ParticipantInfo = $"{pi.Participant.Name}",
                    // If consecutive records have the exact same score and tiebreakers won't work, 
                    Position = index + 1,
                    Score = pi.Score,
                    ScoreInfo = [ new ScoreInfoEntry {
                        IsDiscarded = false,
                        Score = pi.Score,
                        Info = string.Empty
                    }],
                };

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
    }
}