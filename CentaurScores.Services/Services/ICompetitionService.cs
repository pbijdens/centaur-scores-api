﻿using CentaurScores.Model;

namespace CentaurScores.Services
{
    /// <summary>
    /// Competiton-related services
    /// </summary>
    public interface ICompetitionService
    {
        /// <summary>
        /// Calculate the scores for a competition using the applicable ruleset.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        Task<CompetitionResultModel?> CalculateCompetitionResult(int competitionId);
        /// <summary>
        /// Calculate the score for a single match based on its ruleset.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<MatchResultModel?> CalculateSingleMatchResult(int matchId);

        /// <summary>
        /// Returns the ful list of supported rulesets.
        /// </summary>
        /// <returns></returns>
        Task<List<RulesetModel>> GetRulesets();
    }
}