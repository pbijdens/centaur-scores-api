using CentaurScores.CompetitionLogic;
using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using CentaurScoresAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IMatchRepository, MatchRepository>();
builder.Services.AddTransient<ICompetitionRepository, CompetitionRepository>();

builder.AddCompetitionTypes();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<JwtMiddleware>();

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Method.Equals("options", StringComparison.InvariantCultureIgnoreCase) && ctx.Request.Headers.ContainsKey("Access-Control-Request-Private-Network"))
    {
        ctx.Response.Headers["Access-Control-Allow-Private-Network"] = "true";
    }

    await next();
});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

await app.MigrateDatabases();

app.Run();
