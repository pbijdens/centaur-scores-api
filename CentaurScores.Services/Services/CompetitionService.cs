using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CompetitionService(IConfiguration configuration, IEnumerable<IRuleService> ruleServices)
        : ICompetitionService
    {
        /// <inheritdoc/>
        public async Task<CompetitionResultModel?> CalculateCompetitionResult(int competitionId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            IRuleService? applicableRuleService = null;
            db.Database.EnsureCreated();
            CompetitionEntity foundCompetitionEntity = await db.Competitions.FirstOrDefaultAsync(x => x.Id == competitionId) ?? throw new ArgumentException("Bad competition ID", nameof(competitionId));
            foreach (var service in ruleServices)
            {
                if (await service.SupportsCompetition(foundCompetitionEntity))
                {
                    applicableRuleService = service;
                    break;
                }
            }

            // If any service could be found, use it.
            if (applicableRuleService != null)
            {
                return await applicableRuleService.CalculateCompetitionResult(competitionId);
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<List<RulesetModel>> GetRulesets(int? listId)
        {
            List<RulesetModel> result = [];
            foreach (var service in ruleServices)
            {
                result.AddRange(await service.GetSupportedRulesets());
            }

            if (listId != null)
            {
                using var db = new CentaurScoresDbContext(configuration);
                db.Database.EnsureCreated();
                ParticipantListEntity list = db.ParticipantLists.Single(x => x.Id == listId);
                ParticipantListModel listModel = list.ToModel();
                var listConfig = (listModel.Configuration ?? ListConfigurationModel.CentaurIndoorDefaults);
                result = [.. result.Where(x => listConfig.CompetitionFormats.Contains(x.CompetitionFormat))];

                // Update the ruleset suggested match parameters
                foreach (RulesetModel ruleSet in result)
                {
                    ruleSet.RequiredClasses = [.. listConfig.Disciplines.Select(x => new GroupInfo(x))];    // just copy disciplines
                    ruleSet.RequiredSubclasses = [.. listConfig.Divisions.Select(x => new GroupInfo(x))];   // just copy divisions
                    ruleSet.RequiredTargets.RemoveAll(x => !listConfig.Targets.Any(t => t.Code == x.Code)); // delete any targets not supported in this list
                    ruleSet.RequiredScoreValues.Clear();
                    foreach (GroupInfo target in ruleSet.RequiredTargets)
                    {
                        TargetModel? targetConfig = listConfig.Targets.FirstOrDefault(t => t.Code == target.Code);
                        if (null != targetConfig)
                        {
                            ruleSet.RequiredScoreValues[target.Code] = [..targetConfig.Keyboard];
                        }
                    }
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<MatchResultModel?> CalculateSingleMatchResult(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            IRuleService? applicableRuleService = null;
            MatchEntity foundMatchEntity = await db.GetMatchEntitiesUntracked().FirstOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Bad match ID", nameof(id));
            foreach (var service in ruleServices)
            {
                if (await service.SupportsMatch(foundMatchEntity))
                {
                    applicableRuleService = service;
                    break;
                }
            }
            // If any service could be found, use it.
            if (applicableRuleService != null)
            {
                return await applicableRuleService.CalculateSingleMatchResult(id);
            }
            return null;
        }
    }
}
