using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    public class TsbCalculationContext(CentaurScoresDbContext db, List<RulesetModel> supportedRulesets)
    {
        public CentaurScoresDbContext Database => db;

        public List<RulesetModel> SupportedRulesets => supportedRulesets;

        private Dictionary<int, ParticipantListEntryEntity?> ParticipantEntitiesById { get; set; } = [];
        private Dictionary<int, List<CompetitionFormatDisciplineDivisionMapModel>> ParticipantDivisionMappingsById { get; set; } = [];

        public ParticipantListEntryEntity GetParticipantEntityFor(int participantEntityId)
        {
            if (ParticipantEntitiesById.TryGetValue(participantEntityId, out ParticipantListEntryEntity? entity))
            {                 
                return entity!;
            }
            else
            {
                entity = db.ParticipantListEntries.AsNoTracking().FirstOrDefault(x => x.Id == participantEntityId);
                ParticipantEntitiesById[participantEntityId] = entity;
                return entity!;
            }
        }

        public List<CompetitionFormatDisciplineDivisionMapModel> GetCompetitionFormatDisciplineDivisionMapModel(int participantId)
        {
            if (ParticipantDivisionMappingsById.TryGetValue(participantId, out List<CompetitionFormatDisciplineDivisionMapModel>? mappings))
            {
                return mappings;
            }
            else
            {
                ParticipantListEntryEntity? participantListEntry = GetParticipantEntityFor(participantId);
                List<CompetitionFormatDisciplineDivisionMapModel> divisionMappings = System.Text.Json.JsonSerializer.Deserialize<List<CompetitionFormatDisciplineDivisionMapModel>?>(participantListEntry?.CompetitionFormatDisciplineDivisionMapJSON ?? "null") ?? [];
                ParticipantDivisionMappingsById[participantId] = divisionMappings;
                return divisionMappings;
            }
        }
    }
}
