using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace CentaurScores.CompetitionLogic
{
    public class TotalScoreBasedResultCalculatorBase<Tcomparer> where Tcomparer : IComparer<TsbParticipantWrapperSingleMatch>, new()
    {
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

            result.Ungrouped = await SortSingleMatchResult(db, allArrowValues, participants);
            foreach (GroupInfo classGroupInfo in classes)
            {
                result.ByClass[classGroupInfo] = await SortSingleMatchResult(db, allArrowValues, participants.Where(x => x.ClassCode == classGroupInfo.Code).ToList());
                result.BySubclass[classGroupInfo] = new Dictionary<GroupInfo, List<MatchResultEntry>>();
                foreach (GroupInfo subclassGroupInfo in subclasses)
                {
                    result.BySubclass[classGroupInfo][subclassGroupInfo] = await SortSingleMatchResult(db, allArrowValues, participants.Where(x => x.ClassCode == classGroupInfo.Code && x.SubclassCode == subclassGroupInfo.Code).ToList());
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

                if (tiebreakingComparer.Compare(sorted[index - 1], sorted[index]) == 0)
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