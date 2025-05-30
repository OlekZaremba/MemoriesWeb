using System.Collections.Generic;
using System.Threading.Tasks;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public interface IGroupMemberClassRepository
    {
        Task<GroupMemberClass?> GetByIdAsync(int id); 
        Task<GroupMemberClass?> GetFirstByGroupIdAndUserIdAsync(int groupId, int userId); 
        Task<List<GroupMemberClass>> GetByUserGroupIdAsync(int userGroupId); 
        Task<List<GroupMemberClass>> GetAllByGroupMemberIdAsync(int groupMemberId); 
        Task AddAsync(GroupMemberClass entity);

        
        Task<List<GroupMemberClass>> GetAllAssignmentsWithClassAndTeacher(); 
    }
}