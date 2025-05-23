using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoriesBack.DTO;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _groupRepo;

        public UserGroupService(UserGroupRepository groupRepo)
        {
            _groupRepo = groupRepo;
        }

        public async Task<List<GroupDTO>> FindGroupsForTeacherAsync(int teacherId)
        {
            var groups = await _groupRepo.GetGroupsByUserIdAsync(teacherId);
            return groups
                .Select(g => new GroupDTO(g.Id, g.GroupName))
                .ToList();
        }
    }
}
