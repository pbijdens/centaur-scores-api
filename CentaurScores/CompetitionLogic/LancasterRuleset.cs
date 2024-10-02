using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    public class LancasterRuleset : IRuleService
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

        public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
        {
            throw new NotImplementedException();
        }

        public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RulesetModel>> GetSupportedRulesets()
        {
            return await Task.FromResult(RulsetDefinitions);
        }
    }
}
