using Microsoft.AspNetCore.Mvc;
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
            await _assignmentService.AssignTeacherToGroupAsync(teacherId, groupId);
            return Ok("Nauczyciel przypisany do grupy.");
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
    }
}
