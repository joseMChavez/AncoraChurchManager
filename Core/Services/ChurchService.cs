using Core.Models;
using Core.Services.Repository;

namespace Core.Services;
 
    /// <summary>
    /// Business logic related to churches
    /// </summary>
    public class ChurchService(IChurchRepository.IMemberRepository memberRepository, IChurchRepository churchRepository)
    {
        private readonly IChurchRepository churches=churchRepository;
        private readonly  IChurchRepository.IMemberRepository  members=memberRepository;  

        
        /// <summary>
        /// Gets all registered churches
        /// </summary>
        public async Task<List<Church>> GetAllAsync()
        {
            var list = await this.churches.GetAllChurchesAsync();

            // Enrich with member data
            foreach (var church in list)
            {
                church.TotalMembers = await members.CountMembersByChurchAsync(church.Id);
            }

            return list;
        }

        /// <summary>
        /// Gets a church with all associated data
        /// </summary>
        public async Task<(Church church, List<Member> members)> GetDetailsAsync(string id)
        {
            var church =  churches.GetChurchByIdAsync(id);
            var list =  this.members.GetMembersByChurchAsync(id);
            await Task.WhenAll(church, list);
            if (church.Result != null)
                church.Result.TotalMembers = list.Result.Count;

            return (church.Result, list.Result ?? new List<Member>());
        }

        /// <summary>
        /// Creates a new church
        /// </summary>
        public async Task<OperationResult<Church>> CreateAsync(Church church)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(church.Name))
                return OperationResult<Church>.Error("Church name is required");

            if (church.Name.Length > 200)
                return OperationResult<Church>.Error("Church name cannot exceed 200 characters");

            try
            {
                await churches.CreateChurchAsync(church);
                return OperationResult<Church>.Success(church);
            }
            catch (Exception ex)
            {
                return OperationResult<Church>.Error($"Error creating church: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing church
        /// </summary>
        public async Task<OperationResult<Church>> UpdateAsync(Church church)
        {
            if (string.IsNullOrWhiteSpace(church.Id))
                return OperationResult<Church>.Error("Invalid church ID");

            var existing = await churches.GetChurchByIdAsync(church.Id);
            if (existing == null)
                return OperationResult<Church>.Error("Church does not exist");

            try
            {
                await churches.UpdateChurchAsync(church);
                return OperationResult<Church>.Success(church);
            }
            catch (Exception ex)
            {
                return OperationResult<Church>.Error($"Error updating church: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a church and all its members
        /// </summary>
        public async Task<OperationResult<bool>> DeleteAsync(string id)
        {
            var church = await churches.GetChurchByIdAsync(id);
            if (church == null)
                return OperationResult<bool>.Error("Church does not exist");

            try
            {
                await churches.DeleteChurchAsync(id);
                return OperationResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error($"Error deleting church: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets statistics for a church
        /// </summary>
        public async Task<ChurchStatistics> GetStatisticsAsync(string churchId)
        {
            var totalMembers = await members.CountMembersByChurchAsync(churchId);
            var activeMembers = await members.CountActiveMembersByChurchAsync(churchId);

            return new ChurchStatistics
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                InactiveMembers = totalMembers - activeMembers
            };
        }
    }

    // Services/MemberService.cs
    /// <summary>
    /// Business logic related to members
    /// </summary>
    public class MemberService(IChurchRepository.IMemberRepository memberRepository,IChurchRepository churchRepository)
    {
        private readonly  IChurchRepository.IMemberRepository _memberRepository=memberRepository;
        private readonly IChurchRepository _churchRepository=churchRepository;  
        /// <summary>
        /// Gets all members of a church
        /// </summary>
        public async Task<List<Member>> GetMembersByChurchAsync(string churchId)
        {
            return await _memberRepository.GetMembersByChurchAsync(churchId);
        }

        /// <summary>
        /// Gets members filtered by status
        /// </summary>
        public async Task<List<Member>> GetMembersByStatusAsync(string churchId, string status)
        {
            return await _churchRepository.GetMembersByStatusAsync(churchId, status);
        }

        /// <summary>
        /// Creates a new member
        /// </summary>
        public async Task<OperationResult<Member>> CreateAsync(Member member)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(member.ChurchId))
                return OperationResult<Member>.Error("ChurchId is required");

            if (string.IsNullOrWhiteSpace(member.FullName))
                return OperationResult<Member>.Error("Full name is required");

            if (member.FullName.Length > 200)
                return OperationResult<Member>.Error("Full name cannot exceed 200 characters");

            // Verify church exists
            var church = await _churchRepository.GetChurchByIdAsync(member.ChurchId);
            if (church == null)
                return OperationResult<Member>.Error("Specified church does not exist");

            try
            {
                await _memberRepository.CreateMemberAsync(member);
                return OperationResult<Member>.Success(member);
            }
            catch (Exception ex)
            {
                return OperationResult<Member>.Error($"Error creating member: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a member's data
        /// </summary>
        public async Task<OperationResult<Member>> UpdateAsync(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.Id))
                return OperationResult<Member>.Error("Invalid member ID");

            var existing = await _memberRepository.GetMemberByIdAsync(member.Id);
            if (existing == null)
                return OperationResult<Member>.Error("Member does not exist");

            try
            {
                await _memberRepository.UpdateMemberAsync(member);
                return OperationResult<Member>.Success(member);
            }
            catch (Exception ex)
            {
                return OperationResult<Member>.Error($"Error updating member: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a member
        /// </summary>
        public async Task<OperationResult<bool>> DeleteAsync(string id)
        {
            var member = await _memberRepository.GetMemberByIdAsync(id);
            if (member == null)
                return OperationResult<bool>.Error("Member does not exist");

            try
            {
                await _memberRepository.DeleteMemberAsync(id);
                return OperationResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Error($"Error deleting member: {ex.Message}");
            }
        }

        /// <summary>
        /// Search members by name
        /// </summary>
        public async Task<List<Member>> SearchByNameAsync(string churchId, string searchTerm)
        {
            var members = await _memberRepository.GetMembersByChurchAsync(churchId);
            return members
                .Where(m => m.FullName.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    // Models/Helpers/OperationResult.cs
    /// <summary>
    /// Result pattern to return success or error with data
    /// </summary>
    public class OperationResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static OperationResult<T> Success(T data, string message = "Operation successful")
        {
            return new OperationResult<T>
            {
                IsSuccessful = true,
                Data = data,
                Message = message
            };
        }

        public static OperationResult<T> Error(string message)
        {
            return new OperationResult<T>
            {
                IsSuccessful = false,
                Data = default,
                Message = message
            };
        }
    }

    // Models/ChurchStatistics.cs
    public class ChurchStatistics
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
    }
