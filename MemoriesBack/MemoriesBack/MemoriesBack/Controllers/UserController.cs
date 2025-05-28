using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;
using MemoriesBack.Data;
using EntityUser = MemoriesBack.Entities.User;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;



namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly GroupMemberRepository _groupMemberRepo;
        private readonly UserGroupRepository _userGroupRepo;
        private readonly SensitiveDataRepository _sensitiveRepo;

        private readonly AppDbContext _context;

        public UserController(
            UserRepository userRepo,
            GroupMemberRepository groupMemberRepo,
            UserGroupRepository userGroupRepo,
            SensitiveDataRepository sensitiveRepo,
            AppDbContext context) 
        {
            _userRepo = userRepo;
            _groupMemberRepo = groupMemberRepo;
            _userGroupRepo = userGroupRepo;
            _sensitiveRepo = sensitiveRepo;
            _context = context; 
        }


        [HttpPut("{id}/profile-image")]
        public async Task<IActionResult> UploadProfileImage(int id, [FromBody] Dictionary<string, string> body)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new ArgumentException("Nie znaleziono użytkownika");

            if (!body.TryGetValue("image", out var b64) || string.IsNullOrEmpty(b64))
            {
                return BadRequest("Brak obrazu");
            }
            
            var base64Only = b64.Contains("base64,") ? b64.Split(",")[1] : b64;

            try
            {
                _ = Convert.FromBase64String(base64Only); 
                user.Image = b64; 
            }
            catch
            {
                return BadRequest("Nieprawidłowy format obrazu");
            }



            await _userRepo.UpdateAsync(user);
            return Ok();
        }

        [HttpGet("teachers")]
        public async Task<ActionResult<List<UserDTO>>> GetAllTeachers()
        {
            var users = await _userRepo.GetAllAsync();
            var teachers = users
                .Where(u => u.UserRole == EntityUser.Role.T)
                .Select(u => new UserDTO(u.Id, u.Name, u.Surname, u.UserRole))
                .ToList();

            return Ok(teachers);
        }

        [HttpGet("{userId}/groups")]
        public async Task<ActionResult<List<GroupDTO>>> GetGroupsForUser(int userId)
        {
            var members = await _groupMemberRepo.GetAllByUserIdAsync(userId);
            var groupIds = members.Select(m => m.UserGroupId).Distinct().ToList();
            var groups = await _userGroupRepo.GetByIdsAsync(groupIds);

            var dtos = groups
                .Select(g => new GroupDTO(g.Id, g.GroupName))
                .ToList();

            return Ok(dtos);
        }
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "A")
                return Forbid("Dostęp tylko dla administratora.");

            var users = await _userRepo.GetAllAsync();
            var dtos = users
                .Select(u => new UserDTO(u.Id, u.Name, u.Surname, u.UserRole))
                .ToList();

            return Ok(dtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EditUserResponse>> GetUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new ArgumentException("Nie znaleziono użytkownika");

            var sensitive = await _sensitiveRepo.GetByUserAsync(user)
                ?? throw new ArgumentException("Brak danych wrażliwych");

            var imageBase64 = string.IsNullOrWhiteSpace(user.Image) ? "" : user.Image;


            var dto = new EditUserResponse(
                user.Id,
                sensitive.Login,
                user.Name,
                user.Surname,
                user.UserRole,
                imageBase64
            );

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateUser(int id, [FromBody] EditUserRequest req)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new ArgumentException("Nie znaleziono użytkownika");

            user.Name = req.Name;
            user.Surname = req.Surname;
            user.UserRole = req.Role;
            await _userRepo.UpdateAsync(user);

            var sensitive = await _sensitiveRepo.GetByUserAsync(user)
                ?? throw new ArgumentException("Brak danych wrażliwych");

            sensitive.Login = req.Login;
            await _sensitiveRepo.UpdateAsync(sensitive);

            return Ok("Użytkownik zaktualizowany");
        }
        
        [HttpGet("{id}/profile-image")]
        public async Task<IActionResult> GetProfileImage(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound("Nie znaleziono użytkownika");

            var base64 = string.IsNullOrWhiteSpace(user.Image) ? "" : user.Image;

            return Ok(new { image = base64 });
        }
        
        [HttpGet("student/{studentId}/teachers")]
        public async Task<ActionResult<List<UserDTO>>> GetTeachersForStudentAsync(int studentId)
        {
            var studentGroupMembers = await _groupMemberRepo.GetAllByUserIdAsync(studentId);

            var classIds = await _context.GroupMemberClasses
                .Where(gmc => studentGroupMembers.Select(sgm => sgm.Id).Contains(gmc.GroupMemberId))
                .Select(gmc => gmc.SchoolClassId)
                .Distinct()
                .ToListAsync();

            var teacherGroupMemberIds = await _context.GroupMemberClasses
                .Where(gmc => classIds.Contains(gmc.SchoolClassId))
                .Select(gmc => gmc.GroupMember.UserId)
                .Distinct()
                .ToListAsync();

            var teachers = await _userRepo.GetByIdsAsync(teacherGroupMemberIds);

            var dtos = teachers
                .Where(t => t.UserRole == EntityUser.Role.T)
                .Select(t => new UserDTO(t.Id, t.Name, t.Surname, t.UserRole))
                .ToList();

            return Ok(dtos);
        }

    }
}
