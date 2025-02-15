using CentaurScores.CompetitionLogic.TotalScoreBasedLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.CompetitionLogic.CentaurLancasterFormat
{
    /// <summary>Special handler for the indoor lancaster rounds.</summary>
    public class LancasterRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor Lancaster";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoorLancasterQualifier,
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
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoorLancasterFinals,
                    GroupName = GroupName,
                    Code = "LANF",
                    Name = "Lancaster Finaleronde",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 5,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.TargetsLancasterFinale,
                    RequiredScoreValues = RulesetConstants.KeyboardsLancasterFinale
                },
            ];
        private readonly IConfiguration configuration;

        /// <summary>Constructor</summary>
        public LancasterRuleset(IConfiguration configuration)
        {
            this.configuration = configuration;
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = int.MaxValue;
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
        public override async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
