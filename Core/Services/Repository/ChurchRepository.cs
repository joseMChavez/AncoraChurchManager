using Core.Models;

namespace Core.Services.Repository;

public interface IChurchRepository
{
    Task<Church> GetChurchByIdAsync(string id);
    Task<List<Church>> GetAllChurchesAsync();
    Task<int> CreateChurchAsync(Church church);
    Task<int> UpdateChurchAsync(Church church);
    Task<int> DeleteChurchAsync(string id);
   
    Task<List<Member>> GetMembersByStatusAsync(string churchId, string status);
    Task<int> DeleteMembersByChurchAsync(string churchId);
    Task<List<Church>> GetUnsynchronizedChurchesAsync();
    Task<int> MarkChurchSynchronizedAsync(string id);


    public interface IMemberRepository
    {
        Task<Member> GetMemberByIdAsync(string id);
        Task<int> CreateMemberAsync(Member member);
        Task<int> UpdateMemberAsync(Member member);
        Task<int> DeleteMemberAsync(string id);
        Task<int> CountMembersByChurchAsync(string churchId);
        Task<int> CountActiveMembersByChurchAsync(string churchId);
        Task<List<Member>> GetMembersByChurchAsync(string churchId);
        Task<List<Member>> GetUnsynchronizedMembersAsync();
        Task<int> MarkMemberSynchronizedAsync(string id);

    }

    public class ChurchRepository(DatabaseService service) : IChurchRepository, IMemberRepository
    {

        public async Task<List<Church>> GetAllChurchesAsync()
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Church>().ToListAsync();
        }

        public async Task<Church> GetChurchByIdAsync(string id)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Church>()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateChurchAsync(Church church)
        {
            await EnsureConnectionAsync();
            church.UpdateTimestamp();
            return await service._database.InsertAsync(church);
        }

        public async Task<int> UpdateChurchAsync(Church church)
        {
            await EnsureConnectionAsync();
            church.UpdateTimestamp();
            return await service._database.UpdateAsync(church);
        }

        public async Task<int> DeleteChurchAsync(string id)
        {
            await EnsureConnectionAsync();
            // Cascade delete: removes associated members
            await DeleteMembersByChurchAsync(id);
            return await service._database.DeleteAsync<Church>(id);
        }



        public async Task<List<Member>> GetMembersByChurchAsync(string churchId)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => m.ChurchId == churchId)
                .OrderBy(m => m.FullName)
                .ToListAsync();
        }

        public async Task<List<Member>> GetMembersByStatusAsync(string churchId, string status)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => m.ChurchId == churchId && m.Status == status)
                .OrderBy(m => m.FullName)
                .ToListAsync();
        }

        public async Task<Member> GetMemberByIdAsync(string id)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateMemberAsync(Member member)
        {
            await EnsureConnectionAsync();
            member.UpdateTimestamp();
            return await service._database.InsertAsync(member);
        }

        public async Task<int> UpdateMemberAsync(Member member)
        {
            await EnsureConnectionAsync();
            member.UpdateTimestamp();
            return await service._database.UpdateAsync(member);
        }

        public async Task<int> DeleteMemberAsync(string id)
        {
            await EnsureConnectionAsync();
            return await service._database.DeleteAsync<Member>(id);
        }


        public async Task<int> DeleteMembersByChurchAsync(string churchId)
        {
            await EnsureConnectionAsync();
            return await service._database.ExecuteAsync(
                "DELETE FROM Member WHERE ChurchId = ?", churchId);
        }


        public async Task<int> CountMembersByChurchAsync(string churchId)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => m.ChurchId == churchId)
                .CountAsync();
        }

        public async Task<int> CountActiveMembersByChurchAsync(string churchId)
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => m.ChurchId == churchId && m.Status == "Active")
                .CountAsync();
        }

        /// <summary>
        /// Gets all records that need synchronization
        /// </summary>
        public async Task<List<Church>> GetUnsynchronizedChurchesAsync()
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Church>()
                .Where(c => !c.IsSynchronized && c.ShouldSyncToCloud)
                .ToListAsync();
        }

        public async Task<List<Member>> GetUnsynchronizedMembersAsync()
        {
            await EnsureConnectionAsync();
            return await service._database.Table<Member>()
                .Where(m => !m.IsSynchronized && m.ShouldSyncToCloud)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Marks a record as synchronized
        /// </summary>
        public async Task<int> MarkChurchSynchronizedAsync(string id)
        {
            await EnsureConnectionAsync();
            return await service._database.ExecuteAsync(
                $"UPDATE Church SET IsSynchronized = 1 WHERE Id = ?", id);
        }

        public async Task<int> MarkMemberSynchronizedAsync(string id)
        {
            await EnsureConnectionAsync();
            return await service._database.ExecuteAsync(
                "UPDATE Member SET IsSynchronized = 1 WHERE Id = ?", id);
        }



        private async Task EnsureConnectionAsync()
        {
            if (service._database is null)
                await service.InitializeAsync();
        }

        public async Task ClearAllRecordsAsync()
        {
            await EnsureConnectionAsync();
            await service._database.DeleteAllAsync<Member>();
            await service._database.DeleteAllAsync<Church>();
        }

        public async Task CloseConnectionAsync()
        {
            if (service._database != null)
            {
                await service._database.CloseAsync();
                service._database = null;
            }
        }
    }
}