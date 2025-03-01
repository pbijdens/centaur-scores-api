using CentaurScores.CompetitionLogic.TotalScoreBasedLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.CompetitionLogic.StandardCompetition
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
                    CompetitionFormat = RulesetConstants.CompetitionFormat3PFinals,
                    GroupName = GroupName,
                    Code = "3pfinals",
                    Name = "Indoor Finale (3 pijlen)",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 5,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = [],
                    RequiredTargets = [ ..RulesetConstants.Targets18M, ..RulesetConstants.Targets25M ],
                    RequiredScoreValues = RulesetConstants.KeyboardsBoth
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

        /// <summary>Constructor</summary>>
        public Indoor3pRuleset(IConfiguration configuration) : base(configuration)
        {
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = int.MaxValue;
        }

        /// <see cref="IRuleService"/>
        public override async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
