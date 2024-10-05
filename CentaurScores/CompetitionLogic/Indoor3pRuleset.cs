using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    public class Indoor3pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor 18m3p, 25m3p";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "18m3p",
                    Name = "Indoor 18m3p",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets18M,
                    RequiredScoreValues = RulesetConstants.Keyboards18M
                },
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "25m3p",
                    Name = "Indoor 25m3p",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
                },
            };
        private readonly ILogger<ClubkampioenschapIndoor3pRuleset> logger;
        private readonly IConfiguration configuration;

        public Indoor3pRuleset(ILogger<ClubkampioenschapIndoor3pRuleset> logger, IConfiguration configuration)
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
