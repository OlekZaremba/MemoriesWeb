﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;
using MemoriesBack.Data;
using EntityUser = MemoriesBack.Entities.User;

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
                .Select(u => new UserDTO(u.Id, u.Name, u.Surname, u.UserRole.ToString(), null )) 
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
            var users = await _userRepo.GetAllAsync();
            var sensitives = await _sensitiveRepo.GetAllAsync();

            var dtos = users
                .Select(u =>
                {
                    var email = sensitives.FirstOrDefault(s => s.UserId == u.Id)?.Email;
                    return new UserDTO(u.Id, u.Name, u.Surname, u.UserRole.ToString(), email);
                })
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
                sensitive.Email,
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
            sensitive.Email = req.Email;
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
            var studentUser = await _userRepo.GetByIdAsync(studentId);
            if (studentUser == null || studentUser.UserRole != EntityUser.Role.S)
            {
                return NotFound("Student not found or user is not a student.");
            }

            
            var studentGroupMembers = await _groupMemberRepo.GetAllByUserIdAsync(studentId);
            var studentGroupIds = studentGroupMembers.Select(gm => gm.UserGroupId).Distinct().ToList();

            if (!studentGroupIds.Any())
                return Ok(new List<UserDTO>());

            
            var teacherAssignments = await _context.GroupMemberClasses
                .Include(gmc => gmc.GroupMember)
                .ThenInclude(gm => gm.User)
                .Include(gmc => gmc.SchoolClass)
                .Where(gmc =>
                    gmc.GroupMember.User.UserRole == EntityUser.Role.T &&
                    studentGroupIds.Contains(gmc.GroupMember.UserGroupId)
                )
                .ToListAsync();

            var teacherDtos = teacherAssignments
                .Select(gmc => new UserDTO(
                    gmc.GroupMember.User.Id,
                    gmc.GroupMember.User.Name,
                    gmc.GroupMember.User.Surname,
                    gmc.GroupMember.User.UserRole.ToString(),
                    null
                )
                {
                    Subject = gmc.SchoolClass?.ClassName ?? ""
                })
                .DistinctBy(dto => dto.Id) 
                .ToList();

            return Ok(teacherDtos);
        }
        
        public class AssignGroupsRequest
        {
            public List<int> GroupIds { get; set; } = new();
        }

        [HttpGet("{userId}/available-groups")]
        public async Task<ActionResult<List<GroupDTO>>> GetAvailableGroups(int userId)
        {
            var allGroups = await _userGroupRepo.GetAllAsync();
            var assignedGroups = await _groupMemberRepo.GetAllByUserIdAsync(userId);
            var assignedGroupIds = assignedGroups.Select(gm => gm.UserGroupId).ToHashSet();

            var availableGroups = allGroups
                .Where(g => !assignedGroupIds.Contains(g.Id))
                .Select(g => new GroupDTO(g.Id, g.GroupName))
                .ToList();

            return Ok(availableGroups);
        }

        [HttpPost("{userId}/assign-groups")]
        public async Task<IActionResult> AssignGroupsToUser(int userId, [FromBody] AssignGroupsRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            
            if (user.UserRole == EntityUser.Role.S)
            {
                var existing = await _groupMemberRepo.GetAllByUserIdAsync(userId);
                foreach (var gm in existing)
                {
                    _context.GroupMembers.Remove(gm); 
                }

                var firstGroupId = request.GroupIds.FirstOrDefault();
                var group = await _userGroupRepo.GetByIdAsync(firstGroupId);
                if (group != null)
                {
                    var gm = new GroupMember
                    {
                        UserId = userId,
                        UserGroupId = group.Id
                    };
                    await _groupMemberRepo.AddAsync(gm);
                }

                await _context.SaveChangesAsync(); 
                return Ok("Uczeń przypisany do jednej klasy");
            }

            foreach (var groupId in request.GroupIds)
            {
                var group = await _userGroupRepo.GetByIdAsync(groupId);
                if (group == null) continue;

                var exists = await _groupMemberRepo.GetByUserIdAndGroupIdAsync(userId, groupId);
                if (exists != null) continue;

                var gm = new GroupMember
                {
                    UserId = userId,
                    UserGroupId = groupId
                };
                await _groupMemberRepo.AddAsync(gm);
            }

            return Ok("Groups assigned successfully");
        }
    }
}
