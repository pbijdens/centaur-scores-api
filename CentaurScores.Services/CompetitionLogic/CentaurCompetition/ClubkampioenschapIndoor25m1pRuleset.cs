﻿using CentaurScores.CompetitionLogic.TotalScoreBasedLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.CompetitionLogic.CentaurCompetition
{
    /// <summary>Special handler for the 25m 1p Clubkampioenschappen for AHV Centaur.</summary>
    public class ClubkampioenschapIndoor25m1pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Clubkampioenschap Indoor 25m1p";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M1P,
                    GroupName = GroupName,
                    Code = "ck25m1p",
                    Name = "Clubkampioenschap Indoor 25m1p",
                    RequiredArrowsPerEnd = 5,
                    RequiredEnds = 5,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
                },
            ];
        private readonly IConfiguration configuration;

        /// <summary>Constructor</summary>
        public ClubkampioenschapIndoor25m1pRuleset(IConfiguration configuration)
        {
            this.configuration = configuration;
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = 5;
        }

        /// <see cref="IRuleService.CalculateCompetitionResult(int)"></see>
        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            using CentaurScoresDbContext db = new(configuration);
            var result = await CalculateCompetitionResultForDB(db, competitionId);
            return result;
        }

        /// <see cref="IRuleService.CalculateSingleMatchResult(int)"></see>
        public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            using CentaurScoresDbContext db = new(configuration);
            var result = await CalculateSingleMatchResultForDB(db, matchId);
            return result;
        }

        /// <see cref="IRuleService.GetSupportedRulesets()"></see>
        public override async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}

