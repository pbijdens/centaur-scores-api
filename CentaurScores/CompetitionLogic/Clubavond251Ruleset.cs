using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    public class Clubavond251Ruleset : IRuleService
    {
        private const string GroupName = "Indoor 25/1";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "25M1P",
                    Name = "Clubavond 25 Meter 1 Pijl",
                    RequiredArrowsPerEnd = 1,
                    RequiredEnds = 25,
                    RequiredClasses = RulesetConstants.Classes,
                    RequiredSubclasses = RulesetConstants.CentaurSubclassesCompetities,
                    RequiredTargets = RulesetConstants.Targets25M,
                    RequiredScoreValues = RulesetConstants.Keyboards25M
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
