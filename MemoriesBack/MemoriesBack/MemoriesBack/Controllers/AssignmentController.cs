﻿using Microsoft.AspNetCore.Mvc;
using MemoriesBack.Service;
using System.Threading.Tasks;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/assignments")]
    public class AssignmentController : ControllerBase
    {
        private readonly AssignmentService _assignmentService;

        public AssignmentController(AssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost("teacher/{teacherId}/group/{groupId}")]
        public async Task<IActionResult> AssignTeacherToGroup(int teacherId, int groupId)
        {
            try
            {
                await _assignmentService.AssignTeacherToGroupAsync(teacherId, groupId);
                return Ok(new { message = "Nauczyciel przypisany do grupy." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("teacher/{teacherId}/group/{groupId}/class/{classId}")]
        public async Task<IActionResult> AssignTeacherToClass(int teacherId, int groupId, int classId)
        {
            await _assignmentService.AssignTeacherToClassAsync(teacherId, groupId, classId);
            return Ok("Nauczyciel przypisany do przedmiotu w grupie.");
        }

        [HttpGet("teacher/{teacherId}/group/{groupId}")]
        public async Task<IActionResult> GetAssignedClasses(int teacherId, int groupId)
        {
            var classes = await _assignmentService.GetAssignedClassesAsync(teacherId, groupId);
            return Ok(classes.Select(c => new { c.Id, c.ClassName }));
        }
        
        [HttpGet("/api/groups/assignments")]
        public async Task<IActionResult> GetAllAssignments()
        {
            var result = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(result);
        }

    }
}
