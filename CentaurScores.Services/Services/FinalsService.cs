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

            if ((sourceMatch.MatchFlags & MatchEntity.MatchFlagsHeadToHead) != 0)
            {
                MatchModel newMatchResult = await StartFromFinalsDefinition(match, sourceMatch);
                return newMatchResult;
            }
            else
            {
                MatchModel newMatchResult = await CreateFromRegularMatch(id, match, sourceMatch);
                return newMatchResult;
            }
        }

        // Initializes round 1 for a finals definition
        private async Task<MatchModel> StartFromFinalsDefinition(FinalMatchDefinition match, MatchModel sourceMatch)
        {
            if (null == sourceMatch.Competition) throw new ArgumentException("Invalid match identifier - requires competition", "id");
            CompetitionModel competition = sourceMatch.Competition;

            if (null == match.Groups || match.Groups.Count == 0) 
            {
                throw new ArgumentException("Invalid match definition -- requires groups", "match");
            }

            // TODO -> MATCH REPO
            using var db = new CentaurScoresDbContext(configuration);
            MatchEntity matchEntity = await db.GetMatchEntities().Include(m => m.Participants).Where(entity => entity.Id == sourceMatch.Id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(sourceMatch));
            matchEntity.ActiveRound = 1;
            await db.SaveChangesAsync();

            sourceMatch = matchEntity.ToModel();

            List<ParticipantModelFull> participants = await matchRepository.GetParticipantsForMatch(sourceMatch.Id, 0);
            await InitializeFinalsBracketsFromOrderedParticipantList(sourceMatch.Id, match, sourceMatch, 
                (group) => participants.Where(p => p.GroupName == group.Name).Select(p => p.Id).ToList(),
                (id, sp, p) => { return Task.FromResult(sp); }
            );

            return sourceMatch;
        }

        private async Task<MatchModel> CreateFromRegularMatch(int id, FinalMatchDefinition match, MatchModel sourceMatch)
        {
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
            await InitializeFinalsBracketsFromOrderedParticipantList(id, match, newMatchResult,
                getParticipantIdsForGroup: (g) => [.. match.Groups.Find(grp => grp.Name == g.Name)?.Participants.Select(p => p.ParticipantData.Id) ?? []],
                createParticipantModelForMatch: (id, sp, p) => matchRepository.CreateParticipantForMatch(id, p)
                );

            return newMatchResult;
        }

        /// <summary>
        /// Given a match definition and a target match (where the target is a finals match) initializes the target match from an
        /// ordered list of participant identifiers for each of the groups specified in the match definition. Delegates 
        /// for fetching the list of participants per group and for 'creating' the participant in the target match allow for either
        /// using a secondary match to create the finals from, or for simply defining the finals from an already created finals-
        /// match, ultimately allowing the ad-hoc creation of a finals match definition.
        /// </summary>
        private async Task InitializeFinalsBracketsFromOrderedParticipantList(int id, FinalMatchDefinition match, MatchModel newMatchResult, 
            Func<FinalMatchGroupDefinition, List<int>> getParticipantIdsForGroup, 
            Func<int, ParticipantModelSimple, ParticipantModelSimple, Task<ParticipantModelSimple>> createParticipantModelForMatch
            )
        {
            int groupIndex = 0;
            foreach (FinalMatchGroupDefinition group in match.Groups)
            {

                (ParticipantModelSimple? a, ParticipantModelSimple? b)[] bracketDefinitions = new (ParticipantModelSimple? a, ParticipantModelSimple? b)[8];
                Array.Fill(bracketDefinitions, (null, null));

                int[] brackets = [1, 8, 5, 4, 3, 6, 7, 2, 2, 7, 6, 3, 4, 5, 8, 1];
                (int a, int b)[] positionsForBracket = [(1, 16), (8, 9), (5, 12), (4, 13), (3, 14), (6, 11), (7, 10), (2, 15)];

                var participantsForGroup = getParticipantIdsForGroup(group);

                for (int bracketIndex = 0; bracketIndex < 8; bracketIndex++)
                {                    
                    if (participantsForGroup.Count >= positionsForBracket[bracketIndex].a)
                    {
                        ParticipantModelSimple sourceParticipant = await matchRepository.GetParticipantForMatch(id, participantsForGroup[positionsForBracket[bracketIndex].a - 1]);
                        ParticipantModelSimple participant = InitializeParticipantForFinal(newMatchResult, groupIndex, sourceParticipant);
                        bracketDefinitions[bracketIndex].a = await createParticipantModelForMatch(newMatchResult.Id, sourceParticipant, participant);
                    }

                    if (participantsForGroup.Count >= positionsForBracket[bracketIndex].b)
                    {
                        ParticipantModelSimple sourceParticipant = await matchRepository.GetParticipantForMatch(id, participantsForGroup[positionsForBracket[bracketIndex].b - 1]);
                        ParticipantModelSimple participant = InitializeParticipantForFinal(newMatchResult, groupIndex, sourceParticipant);
                        bracketDefinitions[bracketIndex].b = await createParticipantModelForMatch(newMatchResult.Id, sourceParticipant,participant);
                    }

                    ParticipantModelSimple? participantA = (bracketDefinitions[bracketIndex].a == null) ? null : bracketDefinitions[bracketIndex].a;
                    ParticipantModelSimple? participantB = (bracketDefinitions[bracketIndex].b == null) ? null : bracketDefinitions[bracketIndex].b;

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
        }

        private static ParticipantModelSimple InitializeParticipantForFinal(MatchModel newMatch, int groupIndex, ParticipantModelSimple sourceParticipant)
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
            MatchEntity matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));
            MatchModel matchModel = matchEntity.ToModel();

            if (matchEntity.ActiveRound >= 4)
            {
                return;
            }

            IEnumerable<ParticipantModelFull> participantsInThisRound = matchEntity.Participants.Select(m => m.ToFullModel(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, matchEntity.ActiveRound)).Where(m => m.H2HInfo.Count >= matchEntity.ActiveRound);

            foreach (GroupInfo group in matchModel.Groups)
            {
                double numberOfBracketsThisRound = Math.Pow(2, matchEntity.NumberOfRounds - matchEntity.ActiveRound);
                for (int bracketNo = 1; bracketNo <= numberOfBracketsThisRound; bracketNo += 2)
                {
                    ParticipantModelFull? p1 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 0 && p.Group == group.Code);
                    ParticipantModelFull? p2 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 1 && p.Group == group.Code);
                    ParticipantModelFull? p3 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo + 1 && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 0 && p.Group == group.Code);
                    ParticipantModelFull? p4 = participantsInThisRound.SingleOrDefault(p => p.H2HInfo[matchEntity.ActiveRound - 1].Bracket == bracketNo + 1 && p.H2HInfo[matchEntity.ActiveRound - 1].Position == 1 && p.Group == group.Code);

                    ParticipantModelFull? next1 = (null != p1 && p1.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p1 : p2;
                    ParticipantModelFull? next2 = (null != p3 && p3.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p3 : p4;

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
                        ParticipantModelFull? next3 = (null != p1 && p1.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p2 : p1;
                        ParticipantModelFull? next4 = (null != p3 && p3.H2HInfo[matchEntity.ActiveRound - 1].IsWinner) ? p4 : p3;
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
            matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new InvalidOperationException("This should not happen");
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
            MatchEntity matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));
            MatchModel matchModel = matchEntity.ToModel();

            IEnumerable<ParticipantModelFull> participantsInThisRound = matchEntity.Participants.Select(m => m.ToFullModel(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, matchEntity.ActiveRound)).Where(m => m.H2HInfo.Count >= matchEntity.ActiveRound);

            if (matchEntity.ActiveRound >= 4)
            {
                return;
            }

            // Will throw exception if for an actual match no winner was assigned.
            await RegisterWinForAllOpponentlessMatches(matchRepository, matchEntity, participantsInThisRound);

            await db.SaveChangesAsync();
        }

        private async Task AddToNextRound(IMatchRepository matchRepository, MatchEntity matchEntity, int bracketNo, ParticipantModelFull? opponentModel, ParticipantModelFull model, int position)
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

        private async Task RegisterWinForAllOpponentlessMatches(IMatchRepository matchRepository, MatchEntity matchEntity, IEnumerable<ParticipantModelFull> participantsInThisRound)
        {
            foreach (var participant in participantsInThisRound)
            {
                if (!participant.H2HInfo[matchEntity.ActiveRound - 1].IsWinner)
                {
                    ParticipantModelFull? opponent = participantsInThisRound.SingleOrDefault(p => p.Id == participant.H2HInfo[matchEntity.ActiveRound - 1].OpponentId);
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

            MatchEntity matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Competition).Where(entity => entity.Id == matchId).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(matchId));
            MatchModel matchModel = matchEntity.ToModel();

            ParticipantModelFull? winnerModel = db.Participants.FirstOrDefault(p => p.Id == winner)?.ToFullModel(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, matchEntity.ActiveRound);
            ParticipantModelFull? loserModel = db.Participants.FirstOrDefault(p => p.Id == loser)?.ToFullModel(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, matchEntity.ActiveRound);

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
            MatchEntity matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new ArgumentException("Invalid match identifier", nameof(id));

            if (matchEntity.ActiveRound >= 2)
            {
                // update active round
                matchEntity.ActiveRound--;
                await db.SaveChangesAsync();

                // clear all device IDs
                // clear all lijnen
                matchEntity = await db.GetMatchEntitiesUntracked().Include(m => m.Participants).Where(entity => entity.Id == id).SingleOrDefaultAsync() ?? throw new InvalidOperationException("This should not happen");
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
