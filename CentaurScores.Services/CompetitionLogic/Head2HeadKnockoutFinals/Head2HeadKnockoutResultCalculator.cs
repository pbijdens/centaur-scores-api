using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CentaurScores.CompetitionLogic.Head2HeadKnockoutFinals
{
    public class Head2HeadKnockoutResultCalculator(IConfiguration configuration) : IRuleService
    {
        /// <inheritdoc/>
        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            // Won't do competitions
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(new List<RulesetModel>());
        }

        /// <inheritdoc/>
        public async Task<bool> SupportsCompetition(CompetitionEntity competitionEntity)
        {
            return await Task.FromResult(false);
        }

        /// <inheritdoc/>
        public async Task<bool> SupportsMatch(MatchEntity matchEntity)
        {
            return await Task.FromResult((matchEntity.MatchFlags & MatchEntity.MatchFlagsHeadToHead) != 0);
        }

        /// <inheritdoc/>
        public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            using CentaurScoresDbContext db = new(configuration);

            MatchEntity matchEntity = (
                await db.GetMatchEntitiesUntracked()
                    .Where(e => e.Id == matchId)
                    .Include(m => m.Participants)
                .FirstOrDefaultAsync()) ?? throw new ArgumentException("Selected match can't be found");
            MatchModel matchModel = matchEntity.ToModel();

            MatchResultModel result = new MatchResultModel
            {
                // No results
                Ungrouped = [],
                ByClass = [],
                BySubclass = [],
                Code = matchEntity.MatchCode,
                Name = matchEntity.MatchName,
                Flags = matchEntity.MatchFlags,
                Groups = matchModel.Groups,
                Subgroups = [], // no subgroups for finals
                Ruleset = matchEntity.RulesetCode ?? string.Empty,

                // We will populate these
                FinalScores = [],
            };

            await AddBracketsForGroups(db, matchModel, matchEntity, result);
            // for each round
            //   for each discipline
            //     by bracket number
            //       check if there is a winner
            //       but also calculate either the set score or the total score (S or C prefix on group code)

            return result;
        }

        private async Task AddBracketsForGroups(CentaurScoresDbContext db, MatchModel matchModel, MatchEntity matchEntity, MatchResultModel result)
        {
            var groupedResults = matchEntity.Participants.GroupBy(p => p.Group).OrderBy(x => x.Key).ToList();
            foreach (IGrouping<string, ParticipantEntity>? group in groupedResults)
            {
                result.FinalScores[group.Key] = [];

                for (int roundIndex = 1; roundIndex <= 4; roundIndex++)
                {
                    AddRoundScoreForFinalForGroup(matchEntity, result, matchModel.Groups, matchModel.Subgroups, matchModel.Targets, group, roundIndex);
                }

                await Task.FromResult(0);
            }
        }

        private static void AddRoundScoreForFinalForGroup(MatchEntity matchEntity, MatchResultModel result, IEnumerable<GroupInfo> groups, IEnumerable<GroupInfo> subgroups, IEnumerable<GroupInfo> targets, IGrouping<string, ParticipantEntity> group, int roundIndex)
        {
            int[] bracketSizes = [8, 4, 2, 2];

            var matchesThisRound = result.FinalScores[group.Key][roundIndex] = [];

            // Group participants by bracket, ordering them as appropriate
            IEnumerable<IGrouping<int, ParticipantModelV3>> brackets = group
                .Select(p =>
                {
                    var model = p.ToModelV3(groups, subgroups, targets, roundIndex);
                    MatchRepository.AutoFixParticipantModel(matchEntity.ToModel(), model);
                    return model;
                })
                .Where(p => p.H2HInfo != null && p.H2HInfo.Count() >= roundIndex)
                .OrderBy(p => p.H2HInfo[roundIndex - 1].Bracket)
                .ThenBy(p => p.H2HInfo[roundIndex - 1].Position)
                .GroupBy(p => p.H2HInfo[roundIndex - 1].Bracket);

            for (int bracketNo = 1; bracketNo <= bracketSizes[roundIndex - 1]; bracketNo++)
            {
                HeadToHeadScore score = CalculateHeadToHeadScoreForBracketForRound(matchEntity, group, roundIndex, brackets, bracketNo);

                matchesThisRound.Add(score);
            }
        }

        private static HeadToHeadScore CalculateHeadToHeadScoreForBracketForRound(MatchEntity matchEntity, IGrouping<string, ParticipantEntity> group, int roundIndex, IEnumerable<IGrouping<int, ParticipantModelV3>> brackets, int bracketNo)
        {
            HeadToHeadScore score = new() { BracketNumber = bracketNo };
            var bracketData = brackets.SingleOrDefault(b => b.Key == bracketNo);

            if (bracketData != null && bracketData.Any())
            {
                var p1 = bracketData.SingleOrDefault(x => x.H2HInfo[roundIndex - 1].Position == 0);
                var p2 = bracketData.SingleOrDefault(x => x.H2HInfo[roundIndex - 1].Position == 1);

                bool isSetScored = (p1?.H2HInfo[roundIndex - 1].IsSetScored ?? true) && (p2?.H2HInfo[roundIndex - 1].IsSetScored ?? true);

                int?[] endScores1, endScores2;
                CalculateEndscores(matchEntity, roundIndex, p1, p2, isSetScored, out endScores1, out endScores2);

                int endSum1, endSum2;
                CalculateEndSums(matchEntity, group, endScores1, endScores2, isSetScored, out endSum1, out endSum2);

                // winners are always manually assigned
                bool winner1, winner2;
                CalculateWinners(roundIndex, p1, p2, out winner1, out winner2);

                if (null != p1)
                {
                    score.Participant1 = new HeadToHeadMatchStatus()
                    {
                        Participant = p1.ToData(),
                        EndScores = [.. endScores1],
                        Score = endSum1,
                        IsWinner = winner1
                    };
                }
                if (null != p2)
                {
                    score.Participant2 = new HeadToHeadMatchStatus()
                    {
                        Participant = p2.ToData(),
                        EndScores = [.. endScores2],
                        Score = endSum2,
                        IsWinner = winner2
                    };
                }
            }

            return score;
        }

        private static void CalculateEndscores(MatchEntity matchEntity, int roundIndex, ParticipantModelV3? p1, ParticipantModelV3? p2, bool isSetScored, out int?[] endScores1, out int?[] endScores2)
        {
            endScores1 = p1?.Ends.Where(r => r.Round == roundIndex && r.Arrows.Any(a => a != null)).Select(e => e.Score).ToArray() ?? [];
            endScores2 = p2?.Ends.Where(r => r.Round == roundIndex && r.Arrows.Any(a => a != null)).Select(e => e.Score).ToArray() ?? [];
            Array.Resize(ref endScores1, matchEntity.NumberOfEnds);
            Array.Resize(ref endScores2, matchEntity.NumberOfEnds);
        }

        private static void CalculateWinners(int roundIndex, ParticipantModelV3? p1, ParticipantModelV3? p2, out bool winner1, out bool winner2)
        {
            winner1 = p1?.H2HInfo[roundIndex - 1].IsWinner ?? false;
            winner2 = p2?.H2HInfo[roundIndex - 1].IsWinner ?? false;
        }

        private static void CalculateEndSums(MatchEntity matchEntity, IGrouping<string, ParticipantEntity> group, int?[] endScores1, int?[] endScores2, bool isSetScored, out int endSum1, out int endSum2)
        {
            endSum1 = 0;
            endSum2 = 0;
            if (isSetScored)
            {
                for (int i = 0; i < matchEntity.NumberOfEnds; i++)
                {
                    if (endScores1[i] == null && endScores2[i] == null) { }
                    else if (endScores1[i] == endScores2[i]) { endSum1 += 1; endSum2 += 1; }
                    else if (endScores2[i] == null || endScores1[i] > endScores2[i]) { endSum1 += 2; }
                    else { endSum2 += 2; }
                }
            }
            else
            {
                endSum1 = endScores1.Select(x => x ?? 0).Sum();
                endSum2 = endScores2.Select(x => x ?? 0).Sum();
            }
        }
    }
}
