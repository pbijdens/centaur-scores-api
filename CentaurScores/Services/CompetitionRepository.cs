using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Configuration;

namespace CentaurScores.Services
{
    public class CompetitionRepository : ICompetitionRepository
    {
        private readonly IConfiguration configuration;

        public CompetitionRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model)
        {
            ParticipantListEntity participantListEntity = new();
            participantListEntity.UpdateFromModel(model);

            int createdObjectId = -1;
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                EntityEntry<ParticipantListEntity> result = await db.ParticipantLists.AddAsync(participantListEntity);
                await db.SaveChangesAsync();
                createdObjectId = result.Entity?.Id ?? -1;
            }
            return await GetParticipantList(createdObjectId);
        }

        public async Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model)
        {
            int createdObjectId = -1;

            //ParticipantListEntity participantListEntity 
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (participantListEntity == null)
                {
                    throw new ArgumentException(nameof(listId), $"The participant list with that ID does not exist.");
                }

                ParticipantListEntryEntity participantListEntryEntity = new()
                {
                    List = participantListEntity,
                };
                participantListEntryEntity.UpdateFromModel(model);

                EntityEntry<ParticipantListEntryEntity> result = await db.ParticipantListEntries.AddAsync(participantListEntryEntity);
                await db.SaveChangesAsync();
                createdObjectId = result.Entity?.Id ?? -1;
                return await GetParticipantListMember(listId, createdObjectId);
            }
        }

        public async Task<int> DeleteParticipantList(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    db.ParticipantLists.Remove(participantListEntity);
                    await db.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<int> DeleteParticipantListMember(int listId, int memberId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId);
                if (null != participantListEntryEntity)
                {
                    db.ParticipantListEntries.Remove(participantListEntryEntity);
                    await db.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<ParticipantListModel?> GetParticipantList(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    return participantListEntity.ToModel();
                }
            }
            return null;
        }

        public async Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.Id == memberId && x.List.Id == listId);
                if (null != participantListEntryEntity)
                {
                    return participantListEntryEntity.ToModel();
                }
            }
            return null;
        }

        public async Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.Include(x => x.Entries).FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    return participantListEntity.Entries.OrderBy(x => x.Name).ToList().Select(x => x.ToModel()).ToList();
                }
            }
            throw new ArgumentException(nameof(listId), $"A list with that ID does not exist");
        }

        public async Task<List<ParticipantListModel>> GetParticipantLists()
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();
                return await Task.FromResult(db.ParticipantLists.OrderBy(x => x.Name).ToList().Select(x => x.ToModel()).ToList());
            }
        }

        public async Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
                if (null != participantListEntity)
                {
                    participantListEntity.UpdateFromModel(model);
                    await db.SaveChangesAsync();
                    return await GetParticipantList(listId);
                }
            }
            throw new ArgumentException(nameof(listId), $"A list with that ID does not exist");
        }

        public async Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model)
        {
            using (var db = new CentaurScoresDbContext(configuration))
            {
                db.Database.EnsureCreated();

                ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId);
                if (null != participantListEntryEntity)
                {
                    participantListEntryEntity.UpdateFromModel(model);
                    await db.SaveChangesAsync();
                    return await GetParticipantListMember(listId, memberId);
                }
            }
            throw new ArgumentException(nameof(memberId), $"A member with that ID does not exist in that list, or the list does not exist");
        }
    }
}
