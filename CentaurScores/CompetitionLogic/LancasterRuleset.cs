using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    public class LancasterRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor Lancaster";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "LANV",
                    Name = "Lancaster Voorronde",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.TargetsLancaster,
                    RequiredScoreValues = RulesetConstants.KeyboardsLancaster
                },
                new RulesetModel
                {                    
                    GroupName = GroupName,
                    Code = "LANF",
                    Name = "Lancaster Finaleronde",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.TargetsLancasterFinale,
                    RequiredScoreValues = RulesetConstants.KeyboardsLancasterFinale
                },
            };
        private readonly ILogger<LancasterRuleset> logger;
        private readonly IConfiguration configuration;

        public LancasterRuleset(ILogger<LancasterRuleset> logger, IConfiguration configuration)
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
