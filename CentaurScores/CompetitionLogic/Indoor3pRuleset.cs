using CentaurScores.Model;
using CentaurScores.Persistence;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>Special handler for the indoor 3-arrows ruleset.</summary>
    public class Indoor3pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Indoor 18m3p, 25m3p";
        private const string GroupNameFun = "Indoor Fun";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor18M3P,
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
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor18M3P60,
                    GroupName = GroupName,
                    Code = "18m3p60",
                    Name = "Indoor 18m3p 60 pijl",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 20,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets18M,
                    RequiredScoreValues = RulesetConstants.Keyboards18M
                },
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor18M3P120,
                    GroupName = GroupName,
                    Code = "18m3p120",
                    Name = "Indoor 18m3p 120pijl",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 40,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets18M,
                    RequiredScoreValues = RulesetConstants.Keyboards18M
                },
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M3P,
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
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M3P60,
                    GroupName = GroupName,
                    Code = "25m3p60",
                    Name = "Indoor 25m3p 60 pijl",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
                },
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M3P120,
                    GroupName = GroupName,
                    Code = "25m3p120",
                    Name = "Indoor 25m3p 120 pijl",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
                },
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoorFun,
                    GroupName = GroupNameFun,
                    Code = "fun",
                    Name = "Indoor fun-wedstrijd",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.TargetsFun,
                    RequiredScoreValues = RulesetConstants.KeyboardsFun
                },
            ];
        private readonly IConfiguration configuration;

        /// <summary>Constructor</summary>>
        public Indoor3pRuleset(IConfiguration configuration)
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
        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
