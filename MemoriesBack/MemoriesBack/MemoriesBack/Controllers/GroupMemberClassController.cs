using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MemoriesBack.DTO;
using MemoriesBack.Repository;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/groups")]
    public class GroupMemberClassController : ControllerBase
    {
        private readonly GroupMemberClassRepository _repository;

        public GroupMemberClassController(GroupMemberClassRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{groupId}/assignments")]
        public async Task<ActionResult<List<AssignmentDTO>>> GetAssignments(int groupId)
        {
            var groupMemberClasses = await _repository.GetByGroupIdAsync(groupId);
            var result = groupMemberClasses.Select(ToDto).ToList();
            return Ok(result);
        }

        private AssignmentDTO ToDto(Entities.GroupMemberClass gmc)
        {
            var user = gmc.GroupMember.User;
            var schoolClass = gmc.SchoolClass;
            var groupName = gmc.GroupMember.UserGroup?.GroupName ?? "Brak klasy";

            return new AssignmentDTO(
                gmc.Id,
                $"{user.Name} {user.Surname}",     
                schoolClass.ClassName,             
                gmc.GroupMember.UserGroupId,       
                groupName                          
            );
        }

    }
}
