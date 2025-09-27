using CentaurScores.Model;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace CentaurScores.Services
{
    /// <summary>
    /// Competition-related logic 
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class ParticipantListService(IConfiguration configuration) : IParticipantListService
    {
        /// <inheritdoc/>
        public async Task<ParticipantListModel?> CreateParticipantList(ParticipantListModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntity newEntity = new();
            newEntity.UpdateFromModel(model);

            db.Database.EnsureCreated();

            EntityEntry<ParticipantListEntity> createdEntityEntry = await db.ParticipantLists.AddAsync(newEntity);
            await db.SaveChangesAsync();
            int createdObjectId = createdEntityEntry.Entity?.Id ?? -1;
            return await GetParticipantList(createdObjectId);
        }

        /// <inheritdoc/>
        public async Task<ParticipantListMemberModel?> CreateParticipantListMember(int listId, ParticipantListMemberModel model)
        {
            int createdObjectId = -1;

            //ParticipantListEntity participantListEntity 
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId) ?? throw new ArgumentException($"The participant list with that ID does not exist.", nameof(listId));
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

        /// <inheritdoc/>
        public async Task<int> DeactivateParticipantListMember(int listId, int memberId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            ParticipantListEntryEntity foundEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId) ?? throw new ArgumentException("That participant ID does not exist", nameof(memberId));
            foundEntity.IsDeactivated = true;
            await db.SaveChangesAsync();
            return 1;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteParticipantList(int listId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntity? foundEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
            if (null != foundEntity)
            {
                db.ParticipantLists.Remove(foundEntity);
                await db.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteParticipantListMember(int listId, int memberId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntryEntity? foundEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId);
            if (null != foundEntity)
            {
                db.ParticipantListEntries.Remove(foundEntity);
                await db.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        /// <inheritdoc/>
        public async Task<ParticipantListModel?> GetParticipantList(int listId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntity? participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId);
            if (null != participantListEntity)
            {
                return participantListEntity.ToModel();
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<ParticipantListMemberModel?> GetParticipantListMember(int listId, int memberId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntryEntity? participantListEntryEntity = await db.ParticipantListEntries
                .Include(x => x.PersonalBests)
                    .ThenInclude(x => x.List)
                .FirstOrDefaultAsync(x => x.Id == memberId && x.List.Id == listId);
            if (null != participantListEntryEntity)
            {
                return participantListEntryEntity.ToModel();
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task<List<ParticipantListMemberModel>> GetParticipantListMembers(int listId)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntity participantListEntity = await db.ParticipantLists.Include(x => x.Entries).FirstOrDefaultAsync(x => x.Id == listId) ?? throw new ArgumentException($"A list with that ID does not exist", nameof(listId));
            return participantListEntity.Entries.OrderBy(x => x.Name).Where(x => !x.IsDeactivated).ToList().Select(x => x.ToModel()).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<ParticipantListModel>> GetParticipantLists(bool includeInactiveLists = false)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();
            return await Task.FromResult(db.ParticipantLists
                .Where(x => x.IsInactive == null || x.IsInactive == false || includeInactiveLists)
                .OrderBy(x => x.Name)                
                .ToList()
                .Select(x => x.ToModel())
                .ToList());
        }


        /// <inheritdoc/>
        public async Task<ParticipantListModel?> UpdateParticipantList(int listId, ParticipantListModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntity participantListEntity = await db.ParticipantLists.FirstOrDefaultAsync(x => x.Id == listId) ?? throw new ArgumentException($"A list with that ID does not exist", nameof(listId));
            participantListEntity.UpdateFromModel(model);
            await db.SaveChangesAsync();
            return await GetParticipantList(listId);
        }

        /// <inheritdoc/>
        public async Task<ParticipantListMemberModel?> UpdateParticipantListMember(int listId, int memberId, ParticipantListMemberModel model)
        {
            using var db = new CentaurScoresDbContext(configuration);
            db.Database.EnsureCreated();

            ParticipantListEntryEntity participantListEntryEntity = await db.ParticipantListEntries.FirstOrDefaultAsync(x => x.List.Id == listId && x.Id == memberId) ?? throw new ArgumentException($"A member with that ID does not exist in that list, or the list does not exist", nameof(memberId));
            participantListEntryEntity.UpdateFromModel(model);
            await db.SaveChangesAsync();
            return await GetParticipantListMember(listId, memberId);
        }
    }
}
