namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// Use in application building to register competition types.
    /// </summary>
    public static class RulesetRegistrations
    {
        /// <summary>
        /// Use from the app building process to register all supported types of competition rules.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        public static void AddCompetitionTypes(this IHostApplicationBuilder hostApplicationBuilder)
        {
            hostApplicationBuilder.Services.AddSingleton<IRuleService, ClubkampioenschapIndoor3pRuleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, ClubkampioenschapIndoor25m1pRuleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, LancasterRuleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, Indoor3pRuleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, Indoor25m1pRuleset>();
        }
    }
}
