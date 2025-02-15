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
                List<RulesetModel> supportedRulesets = await service.GetSupportedRulesets();
                if (supportedRulesets.Any(x => foundCompetitionEntity.RulesetGroupName != null && x.GroupName == foundCompetitionEntity.RulesetGroupName))
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
        public async Task<List<RulesetModel>> GetRulesets()
        {
            List<RulesetModel> result = [];
            foreach (var service in ruleServices)
            {
                result.AddRange(await service.GetSupportedRulesets());
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<MatchResultModel?> CalculateSingleMatchResult(int id)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            IRuleService? applicableRuleService = null;
            MatchEntity foundMatchEntity = await db.Matches.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Bad match ID", nameof(id));
            foreach (var service in ruleServices)
            {
                List<RulesetModel> supportedRulesets = await service.GetSupportedRulesets();
                if (supportedRulesets.Any(x => x.Code == foundMatchEntity.RulesetCode))
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
