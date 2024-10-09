using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>Special handler for the 25m 1p Clubkampioenschappen for AHV Centaur.</summary>
    public class ClubkampioenschapIndoor3pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Clubkampioenschap Indoor 18m3p, 25m3p";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
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
            ];
        private readonly IConfiguration configuration;

        /// <summary>Constructor</summary>
        public ClubkampioenschapIndoor3pRuleset(IConfiguration configuration)
        {
            this.configuration = configuration;
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = 4; // TODO: Read from the compeetition entity ruleset paramaters!
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
        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
