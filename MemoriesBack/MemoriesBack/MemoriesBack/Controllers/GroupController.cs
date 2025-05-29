using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MemoriesBack.DTO;
using EntityUser = MemoriesBack.Entities.User;
using MemoriesBack.Repository;
using MemoriesBack.Service;
using MemoriesBack.Entities;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly UserRepository _userRepository;
        private readonly UserGroupRepository _userGroupRepository;
        private readonly UserGroupService _userGroupService;
        private readonly GroupMemberClassService _groupMemberClassService;

        public GroupController(
            GroupMemberRepository groupMemberRepository,
            UserRepository userRepository,
            UserGroupRepository userGroupRepository,
            UserGroupService userGroupService,
            GroupMemberClassService groupMemberClassService)
        {
            _groupMemberRepository = groupMemberRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupService = userGroupService;
            _groupMemberClassService = groupMemberClassService;
        }

        [HttpGet("{groupId}/students")]
        public async Task<ActionResult<List<UserDTO>>> GetStudentsInGroup(int groupId)
        {
            var members = await _groupMemberRepository.GetByUserGroupIdWithUsersAsync(groupId);

            var students = members
                .Where(m => m.User != null && m.User.UserRole == EntityUser.Role.S)
                .Select(m => new UserDTO(
                    m.User.Id,
                    m.User.Name,
                    m.User.Surname,
                    m.User.UserRole.ToString()
                ))
                .ToList();

            return Ok(students);
        }


        [HttpPost]
        public async Task<ActionResult<GroupDTO>> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var group = new UserGroup { GroupName = request.GroupName };
            await _userGroupRepository.AddAsync(group);
            return Ok(new GroupDTO(group.Id, group.GroupName));
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDTO>>> GetAllGroups()
        {
            var groups = await _userGroupRepository.GetAllAsync();
            var dtos = groups.Select(g => new GroupDTO(g.Id, g.GroupName)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{groupId}/teachers")]
        public async Task<ActionResult<List<UserDTO>>> GetTeachersInGroup(int groupId)
        {
            var members = await _groupMemberRepository.GetByUserGroupIdAsync(groupId);
            var userIds = members.Select(m => m.UserId).ToList();
            var users = await _userRepository.GetAllAsync();

            var teachers = users
                .Where(u => userIds.Contains(u.Id) && u.UserRole == EntityUser.Role.T)
                .Select(u => new UserDTO(u.Id, u.Name, u.Surname, u.UserRole.ToString()))
                .ToList();

            return Ok(teachers);
        }

        [HttpGet("teacher/{id}")]
        public async Task<ActionResult<List<GroupDTO>>> GetGroupsForTeacher(int id)
        {
            var groups = await _userGroupService.FindGroupsForTeacherAsync(id);
            return Ok(groups);
        }

        [HttpGet("{groupId}/teachers/{teacherId}/subject")]
        public async Task<ActionResult<ClassDTO>> GetSubjectForGroupAndTeacher(int groupId, int teacherId)
        {
            var result = await _groupMemberClassService.FindSubjectByGroupAndTeacherAsync(groupId, teacherId);
            return Ok(result);
        }

		[HttpGet("group/{groupId}/assignments")]
		public async Task<ActionResult<List<AssignmentDTO>>> GetAssignmentsForGroup(int groupId)
		{
    		var assignments = await _groupMemberClassService.GetAssignmentsForGroup(groupId);
    		return Ok(assignments);
		}

    }
}
