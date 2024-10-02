using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    public class ClubavondIndoorDriePijlenRuleset : IRuleService
    {
        private const string GroupName = "Indoor 18m3p, 25m3p";
        private static readonly List<RulesetModel> RulsetDefinitions = new List<RulesetModel>
            {
                new RulesetModel
                {
                    GroupName = GroupName,
                    Code = "18m3p",
                    Name = "Clubavond 18 Meter 3 Pijlen",
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
                    Name = "Clubavond 25 Meter 3 Pijlen",
                    RequiredArrowsPerEnd = 3,
                    RequiredEnds = 10,
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
