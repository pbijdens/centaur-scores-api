using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>Special handler for the 25m 1p indoor ruleset.</summary>
    public class Indoor25m1pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor 25m1p";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M1P,
                    GroupName = GroupName,
                    Code = "25m1p",
                    Name = "Indoor 25m1p",
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
        public Indoor25m1pRuleset(IConfiguration configuration)
        {
            this.configuration = configuration;
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = Int32.MaxValue;
        }

        /// <see cref="IRuleService"/>
        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            using CentaurScoresDbContext db = new(configuration);
            var result = await CalculateCompetitionResultForDB(db, competitionId);
            return result;
        }

        /// <see cref="IRuleService"/>
        public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            using CentaurScoresDbContext db = new(configuration);
            var result = await CalculateSingleMatchResultForDB(db, matchId);
            return result;
        }

        /// <see cref="IRuleService"/>
        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}

