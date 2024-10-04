using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    public class ClubkampioenschapIndoor3pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer>, IRuleService
    {
        private const string GroupName = "Clubkampioenschap Indoor 18m3p, 25m3p";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "ck18m3p",
                    Name = "Clubkampioenschap Indoor 18m3p",
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
                    Code = "ck25m3p",
                    Name = "Clubkampioenschap Indoor 25m3p",
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

        public ClubkampioenschapIndoor3pRuleset(ILogger<ClubkampioenschapIndoor3pRuleset> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            throw new NotImplementedException();
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
