namespace CentaurScores.CompetitionLogic
{
    public static class RulesetRegistrations
    {
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
