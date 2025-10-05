using CentaurScores.CompetitionLogic.TotalScoreBasedLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.CompetitionLogic.CentaurCompetition
{
    /// <summary>Special handler for the 25m 1p Clubkampioenschappen for AHV Centaur.</summary>
    public class ClubkampioenschapIndoor3pRuleset : TotalScoreBasedResultCalculatorBase<TsbTieBreakingComparer, TsbParticipantWrapperCompetitionComparer>, IRuleService
    {
        private const string GroupName = "Clubkampioenschap Indoor 18m3p, 25m3p";
        private static readonly List<RulesetModel> RulsetDefinitions =
            [
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor18M3P,
                    GroupName = GroupName,
                    Code = "ck18m3p",
                    Name = "Clubkampioenschap Indoor 18m3p",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.DefaultClasses,
                    RequiredSubclasses = RulesetConstants.DefaultDivisions,
                    RequiredTargets = RulesetConstants.Targets18M,
                    RequiredScoreValues = RulesetConstants.Keyboards18M
                },
                new RulesetModel
                {
                    CompetitionFormat = RulesetConstants.CompetitionFormatIndoor25M3P,
                    GroupName = GroupName,
                    Code = "ck25m3p",
                    Name = "Clubkampioenschap Indoor 25m3p",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
                    RequiredClasses = RulesetConstants.DefaultClasses,
                    RequiredSubclasses = RulesetConstants.DefaultDivisions,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
                },
            ];

        /// <summary>Constructor</summary>
        public ClubkampioenschapIndoor3pRuleset(IConfiguration configuration) : base(configuration)
        {
            RemoveLowestScoresPerMatchTypeIfMoreThanThisManyMatchesAreAvailableForAParticipant = 5;
        }

        /// <see cref="IRuleService.GetSupportedRulesets()"></see>
        public override async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
