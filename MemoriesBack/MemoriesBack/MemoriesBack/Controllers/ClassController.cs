using Microsoft.AspNetCore.Mvc;
using MemoriesBack.Service;
using MemoriesBack.DTO;
using MemoriesBack.Repository;
using Microsoft.Extensions.Logging;


namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/class")]
    public class ClassController : ControllerBase
    {
        private readonly GradeService _gradeService;
        private readonly ILogger<ClassController> _logger;
        private readonly GroupMemberClassRepository _groupMemberClassRepository;

        public ClassController(
            GradeService gradeService,
            ILogger<ClassController> logger,
            GroupMemberClassRepository groupMemberClassRepository)
        {
            _gradeService = gradeService;
            _logger = logger;
            _groupMemberClassRepository = groupMemberClassRepository;
        }

        [HttpGet("student/{userId}/subjects")]
        public async Task<IActionResult> GetSubjectsForStudent(int userId)
        {
            var subjects = await _gradeService.GetSubjectsForStudentAsync(userId);
            return Ok(subjects);
        }
        
        [HttpGet("{classId}/teachers")]
        public async Task<IActionResult> GetTeachersByClassId(int classId)
        {
            var data = await _groupMemberClassRepository.GetFullInfoByClassIdAsync(classId);

            var result = data.Select(gmc => new
            {
                teacherId = gmc.GroupMember.User.Id,
                teacherName = $"{gmc.GroupMember.User.Name} {gmc.GroupMember.User.Surname}",
                groupId = gmc.GroupMember.UserGroup.Id,
                groupName = gmc.GroupMember.UserGroup.GroupName
            });

            return Ok(result);
        }
    }
}
