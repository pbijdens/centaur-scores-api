namespace CentaurScores.CompetitionLogic
{
    public static class RulesetRegistrations
    {
        public static void AddCompetitionTypes(this IHostApplicationBuilder hostApplicationBuilder)
        {
            hostApplicationBuilder.Services.AddSingleton<IRuleService, ClubavondIndoorDriePijlenRuleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, Clubavond251Ruleset>();
            hostApplicationBuilder.Services.AddSingleton<IRuleService, LancasterRuleset>();
        }
    }
}
