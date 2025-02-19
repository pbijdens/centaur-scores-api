using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CentaurScores.Services
{
    public class FinalsService(IMatchRepository matchRepository, IConfiguration configuration)
        : IFinalsService
    {
        /// <inheritdoc/>
        public async Task<MatchModel> CreateFromMatch(int id, FinalMatchDefinition match)
        {
            MatchModel sourceMatch = await matchRepository.GetMatch(id);
            if (null == sourceMatch) throw new ArgumentException("Invalid match identifier", nameof(id));

            if (null == sourceMatch.Competition) throw new ArgumentException("Invalid match identifier - requires competition", nameof(id));
            CompetitionModel competition = sourceMatch.Competition;

            MatchModel newMatch = new MatchModel
            {
                ActiveRound = 1,
                NumberOfRounds = 4,
                Competition = competition,
                ArrowsPerEnd = sourceMatch.ArrowsPerEnd,
                NumberOfEnds = 5,
                Groups = new Func<List<GroupInfo>>(() =>
                {
                    List<GroupInfo> groups = [];
                    int nextId = 0;
                    foreach (FinalMatchGroupDefinition group in match.Groups)
                    {
                        char suffix = group.IsSetScored ? 'S' : 'C';
                        groups.Add(new GroupInfo
                        {
                            Code = $"{nextId}-{suffix}",
                            Id = nextId,
                            Label = group.Name
                        });
                        nextId++;
                    }
                    return groups;
                }).Invoke(),
                Id = -1,
                IsActiveMatch = false,
                Lijnen = new List<string> { "A", "B", "C", "D" },
                MatchCode = $"{DateTime.Now.ToString("O").Substring(0, 10)}",
                MatchName = match.Name,
                MatchFlags = MatchEntity.MatchFlagsHeadToHead,
                Subgroups = [],
                RulesetCode = sourceMatch.RulesetCode,
                Targets = sourceMatch.Targets,
                ScoreValues = sourceMatch.ScoreValues,
            };

            MatchModel newMatchResult = await matchRepository.CreateMatch(newMatch);
            int groupIndex = 0;
            foreach (FinalMatchGroupDefinition group in match.Groups)
            {

                (ParticipantModel? a, ParticipantModel? b)[] bracketDefinitions = new (ParticipantModel? a, ParticipantModel? b)[8];
                Array.Fill(bracketDefinitions, (null, null));

                int[] brackets = [1, 8, 5, 4, 3, 6, 7, 2, 2, 7, 6, 3, 4, 5, 8, 1];
                (int a, int b)[] positionsForBracket = [(1, 16), (8, 9), (5, 12), (4, 13), (3, 14), (6, 11), (7, 10), (2, 15)];

                for (int bracketIndex = 0; bracketIndex < 8; bracketIndex++)
                {
                    if (group.Participants.Count >= positionsForBracket[bracketIndex].a)
                    {
                        ParticipantModel sourceParticipant = await matchRepository.GetParticipantForMatch(id, group.Participants[positionsForBracket[bracketIndex].a - 1].ParticipantData.Id);
                        ParticipantModel participant = InitializeParticipantForFinal(newMatch, groupIndex, sourceParticipant);
                        bracketDefinitions[bracketIndex].a = await matchRepository.CreateParticipantForMatch(newMatchResult.Id, participant);
                    }

                    if (group.Participants.Count >= positionsForBracket[bracketIndex].b)
                    {
                        ParticipantModel sourceParticipant = await matchRepository.GetParticipantForMatch(id, group.Participants[positionsForBracket[bracketIndex].b - 1].ParticipantData.Id);
                        ParticipantModel participant = InitializeParticipantForFinal(newMatch, groupIndex, sourceParticipant);
                        bracketDefinitions[bracketIndex].b = await matchRepository.CreateParticipantForMatch(newMatchResult.Id, participant);
                    }

                    ParticipantModel? participantA = (bracketDefinitions[bracketIndex].a == null) ? null : bracketDefinitions[bracketIndex].a;
                    ParticipantModel? participantB = (bracketDefinitions[bracketIndex].b == null) ? null : bracketDefinitions[bracketIndex].b;

                    HeadToHeadInfoEntry entryA = new HeadToHeadInfoEntry
                    {
                        Bracket = bracketIndex + 1,
                        InitialPosition = positionsForBracket[bracketIndex].a,
                        IsWinner = participantB == null,
                        Position = 0,
                        OpponentId = participantB?.Id ?? -1,
                        IsSetScored = group.IsSetScored,
                    };
                    if (null != participantA) await matchRepository.UpdateParticipantHeadToHeadInfo(participantA, [entryA]);

                    HeadToHeadInfoEntry entryB = new HeadToHeadInfoEntry
                    {
                        Bracket = bracketIndex + 1,
                        InitialPosition = positionsForBracket[bracketIndex].b,
                        IsWinner = participantA == null,
                        Position = 1,
                        OpponentId = participantA?.Id ?? -1,
                        IsSetScored = group.IsSetScored,
                    };

                    if (null != participantB) await matchRepository.UpdateParticipantHeadToHeadInfo(participantB, [entryB]);
                }
                groupIndex++;
            }

            return newMatchResult;
        }

        private static ParticipantModel InitializeParticipantForFinal(MatchModel newMatch, int groupIndex, ParticipantModel sourceParticipant)
        {
            return new()
            {
                Group = newMatch.Groups[groupIndex].Code,
                HeadToHeadJSON = "[]",
                Name = sourceParticipant.Name,
                Target = sourceParticipant?.Target ?? "",
                ParticipantListEntryId = sourceParticipant?.ParticipantListEntryId,
                Score = 0,
                Ends = [],
                Id = -1
            };
        }

        public async Task GotoNextRound(int id)
        {
            // In a separate call in a separate database session, calculate the winners of matches with a bye
            await CalculateClearWinners(id);

            // Open a new context now and process the next round
            using var db = new CentaurScoresDbContext(configuration);
            MatchEntity matchEntity = await db.Matches.Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));

            if (matchEntity.ActiveRound >= 4)
            {
                return;
            }

            GroupInfo[] groups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.GroupsJSON ?? "[]") ?? [];
            GroupInfo[] subgroups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.SubgroupsJSON ?? "[]") ?? [];
            GroupInfo[] targets = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.TargetsJSON ?? "[]") ?? [];

            IEnumerable<ParticipantModelV3> participantsInThisRound = matchEntity.Participants.Select(m => m.ToModelV3(groups, subgroups, targets, matchEntity.ActiveRound)).Where(m => m.H2HInfo.Count >= matchEntity.ActiveRound);

            foreach (GroupInfo group in groups)
            {
                double numberOfBracketsThisRound = Math.Pow(2, matchEntity.NumberOfRounds - matchEntity.ActiveRound);
                for (int bracketNo = 1; bracketNo <= numberOfBracketsThisRound; bracketNo += 2)
                {
                    ParticipantModelV3? p1 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 0 && p.Group == group.Code);
                    ParticipantModelV3? p2 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 1 && p.Group == group.Code);
                    ParticipantModelV3? p3 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo + 1 && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 0 && p.Group == group.Code);
                    ParticipantModelV3? p4 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo + 1 && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 1 && p.Group == group.Code);

                    ParticipantModelV3? next1 = (null != p1 && p1.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p1 : p2;
                    ParticipantModelV3? next2 = (null != p3 && p3.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p3 : p4;

                    if (null != next1)
                    {
                        await AddToNextRound(matchRepository, matchEntity, bracketNo, next2, next1, 0);
                    }

                    if (null != next2)
                    {
                        await AddToNextRound(matchRepository, matchEntity, bracketNo, next1, next2, 1);
                    }

                    if (matchEntity.ActiveRound == matchEntity.NumberOfRounds - 1) // add losers-final
                    {
                        ParticipantModelV3? next3 = (null != p1 && p1.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p2 : p1;
                        ParticipantModelV3? next4 = (null != p3 && p3.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p4 : p3;
                        if (null != next3)
                        {
                            await AddToNextRound(matchRepository, matchEntity, bracketNo + 2, next4, next3, 0);
                        }

                        if (null != next4)
                        {
                            await AddToNextRound(matchRepository, matchEntity, bracketNo + 2, next3, next4, 1);
                        }
                    }
                }
            }

            // update active round
            matchEntity.ActiveRound++;
            await db.SaveChangesAsync();

            // clear all device IDs
            // clear all lijnen
            matchEntity = await db.Matches.Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new InvalidOperationException("This should not happen");
            foreach (ParticipantEntity participant in matchEntity.Participants.ToList())
            {
                participant.Lijn = string.Empty;
                participant.DeviceID = string.Empty;
            }
            await db.SaveChangesAsync();
        }

        private async Task CalculateClearWinners(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            MatchEntity matchEntity = await db.Matches.Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));

            GroupInfo[] groups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.GroupsJSON ?? "[]") ?? [];
            GroupInfo[] subgroups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.SubgroupsJSON ?? "[]") ?? [];
            GroupInfo[] targets = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.TargetsJSON ?? "[]") ?? [];

            IEnumerable<ParticipantModelV3> participantsInThisRound = matchEntity.Participants.Select(m => m.ToModelV3(groups, subgroups, targets, matchEntity.ActiveRound)).Where(m => m.H2HInfo.Count >= matchEntity.ActiveRound);

            if (matchEntity.ActiveRound >= 4)
            {
                return;
            }

            // Will throw exception if for an actual match no winner was assigned.
            await RegisterWinForAllOpponentlessMatches(matchRepository, matchEntity, participantsInThisRound);

            await db.SaveChangesAsync();
        }

        private async Task AddToNextRound(IMatchRepository matchRepository, MatchEntity matchEntity, int bracketNo, ParticipantModelV3? opponentModel, ParticipantModelV3 model, int position)
        {
            Console.WriteLine($"Adding in bracket {bracketNo}:{position} archer {model.Name}@{model.Id} vs {opponentModel?.Name}@{opponentModel?.Id}");

            // reduce
            while (model.H2HInfo.Count > matchEntity.ActiveRound)
            {
                model.H2HInfo.RemoveAt(model.H2HInfo.Count - 1);
            }
            model.H2HInfo.Add(new HeadToHeadInfoEntry
            {
                Bracket = 1 + ((bracketNo - 1) / 2),
                InitialPosition = model.H2HInfo[matchEntity.ActiveRound - 1].InitialPosition,
                IsSetScored = model.H2HInfo[matchEntity.ActiveRound - 1].IsSetScored,
                Position = position,
                IsWinner = opponentModel == null,
                OpponentId = opponentModel?.Id ?? -1,                
            });
            await matchRepository.UpdateParticipantHeadToHeadInfo(model, model.H2HInfo.ToArray());
        }

        private async Task RegisterWinForAllOpponentlessMatches(IMatchRepository matchRepository, MatchEntity matchEntity, IEnumerable<ParticipantModelV3> participantsInThisRound)
        {
            foreach (var participant in participantsInThisRound)
            {
                if (!participant.H2HInfo[matchEntity.ActiveRound - 1].IsWinner)
                {
                    ParticipantModelV3? opponent = participantsInThisRound.SingleOrDefault(p => p.Id == participant.H2HInfo[matchEntity.ActiveRound - 1].OpponentId);
                    if (null == opponent)
                    {
                        participant.H2HInfo[matchEntity.ActiveRound - 1].IsWinner = true;
                        await matchRepository.UpdateParticipantHeadToHeadInfo(participant, participant.H2HInfo.ToArray());
                    }
                    else if (!opponent.H2HInfo[matchEntity.ActiveRound - 1].IsWinner)
                    {
                        throw new InvalidOperationException("Please make sure that all matches have a winner.");
                    }
                }
            }
        }

        public async Task UpdateFinalsBracketWinner(int matchId, string discipline, int bracket, int winner, int loser)
        {
            using var db = new CentaurScoresDbContext(configuration);

            MatchEntity matchEntity = await db.Matches.Include(m => m.Competition).AsNoTracking().Where(entity => entity.Id == matchId).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(matchId));

            GroupInfo[] groups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.GroupsJSON ?? "[]") ?? [];
            GroupInfo[] subgroups = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.SubgroupsJSON ?? "[]") ?? [];
            GroupInfo[] targets = JsonConvert.DeserializeObject<GroupInfo[]>(matchEntity.TargetsJSON ?? "[]") ?? [];

            ParticipantModelV3? winnerModel = db.Participants.FirstOrDefault(p => p.Id == winner)?.ToModelV3(groups, subgroups, targets, matchEntity.ActiveRound);
            ParticipantModelV3? loserModel = db.Participants.FirstOrDefault(p => p.Id == loser)?.ToModelV3(groups, subgroups, targets, matchEntity.ActiveRound);

            if (winnerModel != null)
            {
                winnerModel.H2HInfo[matchEntity.ActiveRound - 1].IsWinner = true;
                await matchRepository.UpdateParticipantHeadToHeadInfo(winnerModel, winnerModel.H2HInfo.ToArray());
            }
            if (loserModel != null)
            {
                loserModel.H2HInfo[matchEntity.ActiveRound - 1].IsWinner = false;
                await matchRepository.UpdateParticipantHeadToHeadInfo(loserModel, loserModel.H2HInfo.ToArray());
            }
        }

        /// <inheritdoc/>
        public async Task GotoPreviousRound(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            MatchEntity matchEntity = await db.Matches.Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));

            if (matchEntity.ActiveRound >= 2)
            {
                // update active round
                matchEntity.ActiveRound--;
                await db.SaveChangesAsync();

                // clear all device IDs
                // clear all lijnen
                matchEntity = await db.Matches.Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new InvalidOperationException("This should not happen");
                foreach (ParticipantEntity participant in matchEntity.Participants.ToList())
                {
                    participant.Lijn = string.Empty;
                    participant.DeviceID = string.Empty;
                }
                await db.SaveChangesAsync();
            }
        }

    }
}
