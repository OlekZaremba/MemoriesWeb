using Microsoft.AspNetCore.Mvc;
using MemoriesBack.Service;
using MemoriesBack.DTO;
using Microsoft.Extensions.Logging;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/class")]
    public class ClassController : ControllerBase
    {
        private readonly GradeService _gradeService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(GradeService gradeService, ILogger<ClassController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }

        [HttpGet("student/{userId}/subjects")]
        public async Task<IActionResult> GetSubjectsForStudent(int userId)
        {
            var subjects = await _gradeService.GetSubjectsForStudentAsync(userId);
            return Ok(subjects);
        }
    }
}
