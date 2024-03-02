using CentaurScores.Model;
using CentaurScores.Persistence;
using CentaurScores.Services;
using CentaurScoresAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
