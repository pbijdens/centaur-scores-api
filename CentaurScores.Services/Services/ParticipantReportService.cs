using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.Services;

public class ParticipantReportService(IConfiguration configuration, IEnumerable<IRuleService> ruleServices) : IParticipantReportService
{
    public async Task<List<ParticipantReport>> GetParticipantReportPerDiscipline(int listId, bool activeOnly = true)
    {
        List<ParticipantReport> result = [];

        using var db = new CentaurScoresDbContext(configuration);
        db.Database.EnsureCreated();

        ParticipantListEntity participantListEntity = await db.ParticipantLists.Include(x => x.Entries).FirstOrDefaultAsync(x => x.Id == listId) ?? throw new ArgumentException($"A list with that ID does not exist", nameof(listId));
        var plModel = participantListEntity.ToModel();

        DisciplineModel[] disciplines = (plModel.Configuration?.Disciplines ?? []).OrderBy(x => x.Label).ToArray();

        foreach (DisciplineModel discipline in disciplines)
        {
            result.Add(await CreateReportForSingleDiscipline(db, plModel, discipline, activeOnly));
        }

        return result;
    }

    private async Task<ParticipantReport> CreateReportForSingleDiscipline(CentaurScoresDbContext db, ParticipantListModel plModel, DisciplineModel discipline, bool activeOnly)
    {
        List<RulesetModel> rulesets = await GetRulesets();

        ParticipantReport result = new ParticipantReport { Discipline = discipline.Label };

        IOrderedQueryable<CompetitionEntity> competitionsQuery = db.Competitions.AsNoTracking()
            .Where(c => c.ParticipantList != null && c.ParticipantList.Id == plModel.Id)
            .Where(c => (!activeOnly || (c.IsInactive != true)))
            .Include(c => c.Matches)
            .ThenInclude(m => m.Participants)
            .OrderBy(c => c.StartDate)
            .ThenBy(c => c.Name)
            .ThenBy(c => c.Id)
            ;

        result.ActiveCompetitions = await competitionsQuery.Select(c => c.Name).ToListAsync();

        int index = 0; ;
        foreach (CompetitionEntity? competitionEntity in competitionsQuery.ToList())
        {
            foreach (MatchEntity match in competitionEntity.Matches.ToList())
            {
                MatchModel matchModel = match.ToModel();

                foreach (ParticipantEntity participant in (match.Participants.Where(p => p.Group == discipline.Code) ?? []))
                {
                    if (participant.ParticipantListEntryId == null || participant.ParticipantListEntryId < 0) continue;

                    ParticipantListEntryEntity? member = await db.ParticipantListEntries.FirstOrDefaultAsync(p => p.Id == participant.ParticipantListEntryId);
                    if (member == null) continue;
                    ParticipantListMemberModel memberModel = member.ToModel();

                    ParticipantReport.ParticipantReportEntry? participantReportEntry = AddParticipantReportEntryIfNeeded(result, competitionsQuery, participant, memberModel);
                    ParticipantReport.ParticipantCompetitionReportEntry competitionReportEntry = GetOrAddCompetitionReportEntry(discipline, index, competitionEntity, memberModel, participantReportEntry);

                    ParticipantModelFull participantModel = participant.ToFullModel(matchModel.Groups, matchModel.Subgroups, matchModel.Targets, match.ActiveRound);

                    competitionReportEntry.MatchesPlayed++;
                    competitionReportEntry.ArrowTotal += participant.Score;
                    competitionReportEntry.ArrowsShot += participantModel.Ends.Sum(e => e.Arrows.Where(x => e.Round == 0).Count());
                    if (competitionReportEntry.Division == null)
                    {
                        string? divisionCode = memberModel.CompetitionFormatDisciplineDivisionMap?.FirstOrDefault(x =>
                            competitionEntity.Id == x.CompetitionID &&
                            discipline.Code == x.DisciplineCode
                            )?.DivisionCode;
                        competitionReportEntry.Division = plModel.Configuration?.Divisions.FirstOrDefault(d => d.Code == divisionCode)?.Label ?? "Onbekend";
                    }
                }
            }
            index++;
        }

        var competitions = await competitionsQuery.ToListAsync();
        foreach (ParticipantListMemberModel part in plModel.Entries)
        {
            if (!result.Participants.Any(p => p.MemberId == part.Id))
            {
                CompetitionFormatDisciplineDivisionMapModel[] classDefs = part.CompetitionFormatDisciplineDivisionMap.Where(x => x.DisciplineCode == discipline.Code && !string.IsNullOrEmpty(x.DivisionCode) && competitions.Any(c => x.CompetitionID == c.Id && c.IsInactive != true)).ToArray();

                if (classDefs.Length != 0)
                {
                    var participantReportEntry = new ParticipantReport.ParticipantReportEntry
                    {
                        MemberId = part.Id ?? -1,
                        Name = part.Name,
                        Competitions = [.. competitions.Select(c => 
                        {
                            if (classDefs.Any(cd => cd.CompetitionID == c.Id))
                            {
                                string divisionCode = classDefs.First(cd => cd.CompetitionID == c.Id).DivisionCode ?? string.Empty;
                                return new ParticipantReport.ParticipantCompetitionReportEntry
                                {
                                    Division = plModel.Configuration?.Divisions.FirstOrDefault(d => d.Code == divisionCode)?.Label ?? "Onbekend",
                                    MatchesPlayed = 0,
                                    PerArrowAverage = 0.0,
                                    ArrowsShot = 0,
                                    ArrowTotal = 0
                                };
                            }
                            else
                            {
                                return null;
                            }
                        })]
                    };
                    result.Participants.Add(participantReportEntry);
                }
            }
        }

        foreach (var line in result.Participants)
        {
            foreach (var comp in line.Competitions)
            {
                if (comp != null && comp.ArrowsShot > 0)
                {
                    comp.PerArrowAverage = (double)comp.ArrowTotal / (double)comp.ArrowsShot;
                }
            }
        }

        result.Participants = [.. result.Participants.OrderBy(x => x.Name)];

        return result;
    }

    private static ParticipantReport.ParticipantCompetitionReportEntry GetOrAddCompetitionReportEntry(DisciplineModel discipline, int index, CompetitionEntity c, ParticipantListMemberModel memberModel, ParticipantReport.ParticipantReportEntry participantReportEntry)
    {
        ParticipantReport.ParticipantCompetitionReportEntry? competitionReportEntry = participantReportEntry.Competitions[index];
        if (null == competitionReportEntry)
        {
            competitionReportEntry = new ParticipantReport.ParticipantCompetitionReportEntry
            {
                Division = null,
                MatchesPlayed = 0,
                PerArrowAverage = 0.0,
                ArrowsShot = 0,
                ArrowTotal = 0
            };
            participantReportEntry.Competitions[index] = competitionReportEntry;
        }
        return competitionReportEntry;
    }

    private static ParticipantReport.ParticipantReportEntry AddParticipantReportEntryIfNeeded(ParticipantReport result, IOrderedQueryable<CompetitionEntity> competitionsQuery, ParticipantEntity participant, ParticipantListMemberModel memberModel)
    {
        var participantReportEntry = result.Participants.FirstOrDefault(p => p.MemberId == memberModel.Id);
        if (participantReportEntry == null)
        {
            participantReportEntry = new ParticipantReport.ParticipantReportEntry
            {
                MemberId = memberModel.Id ?? -1,
                Name = memberModel.Name,
                Competitions = [.. Enumerable.Range(0, competitionsQuery.Count()).Select(_ => (ParticipantReport.ParticipantCompetitionReportEntry?)null)]
            };
            result.Participants.Add(participantReportEntry);
        }

        return participantReportEntry;
    }

    private async Task<List<RulesetModel>> GetRulesets()
    {
        List<RulesetModel> result = [];
        foreach (var service in ruleServices)
        {
            result.AddRange(await service.GetSupportedRulesets());
        }
        return result;
    }

}
