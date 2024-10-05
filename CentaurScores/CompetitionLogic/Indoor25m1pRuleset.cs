using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace CentaurScores.CompetitionLogic
{
    public class Indoor25m1pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor 25m1p";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
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
            };
        private readonly ILogger<ClubkampioenschapIndoor25m1pRuleset> logger;
        private readonly IConfiguration configuration;

        public Indoor25m1pRuleset(ILogger<ClubkampioenschapIndoor25m1pRuleset> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                var result = await CalculateCompetitionResultForDB(db, competitionId);
                return result;
            }
        }

        public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                var result = await CalculateSingleMatchResultForDB(db, matchId);
                return result;
            }
        }

        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}

