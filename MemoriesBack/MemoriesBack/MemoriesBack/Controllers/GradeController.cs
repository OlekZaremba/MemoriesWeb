using Microsoft.AspNetCore.Mvc;
using MemoriesBack.DTO;
using MemoriesBack.Service;
using Microsoft.Extensions.Logging;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/grades")]
    public class GradeController : ControllerBase
    {
        private readonly GradeService _gradeService;
        private readonly ILogger<GradeController> _logger;

        public GradeController(GradeService gradeService, ILogger<GradeController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade([FromBody] GradeRequest req)
        {
            await _gradeService.AddGradeAsync(req);
            return StatusCode(201);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetGradesForStudent(int studentId)
        {
            var grades = await _gradeService.GetAllGradesForStudentAsync(studentId);
            return Ok(grades);
        }

        [HttpGet("student/{studentId}/subject/{subjectId}")]
        public async Task<IActionResult> GetGradesForSubject(int studentId, int subjectId)
        {
            var grades = await _gradeService.GetGradesForSubjectAsync(studentId, subjectId);
            return Ok(grades);
        }

        [HttpGet("{gradeId}")]
        public async Task<IActionResult> GetGradeDetails(int gradeId)
        {
            var details = await _gradeService.GetGradeDetailsAsync(gradeId);
            return Ok(details);
        }

        [HttpGet("student/{studentId}/new")]
        public async Task<IActionResult> GetNewGrades(int studentId)
        {
            var newGrades = await _gradeService.GetNewGradesForStudentAsync(studentId);
            return Ok(newGrades);
        }

        [HttpGet("teacher/{teacherId}/classes")]
        public async Task<IActionResult> GetClassesForTeacher(int teacherId)
        {
            var result = await _gradeService.GetClassesForTeacherAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("teacher/{teacherId}/groups")]
        public async Task<IActionResult> GetGroupsForTeacher(int teacherId)
        {
            var result = await _gradeService.GetGroupsForTeacherAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("group/{groupId}/students")]
        public async Task<IActionResult> GetStudentsForGroup(int groupId)
        {
            var students = await _gradeService.GetStudentsForGroupAsync(groupId);
            return Ok(students);
        }
        
        [HttpGet("student/{studentId}/subjects")]
        public async Task<IActionResult> GetSubjectsForStudent(int studentId)
        {
            var subjects = await _gradeService.GetSubjectsForStudentAsync(studentId);
            return Ok(subjects);
        }

    }
}
