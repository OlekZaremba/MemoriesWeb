using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;
using MemoriesBack.Data;
using AppUser = MemoriesBack.Entities.User;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/classes")]
    public class SchoolClassController : ControllerBase
    {
        private readonly SchoolClassRepository _schoolClassRepository;
        private readonly AppDbContext _context;

        public SchoolClassController(SchoolClassRepository schoolClassRepository, AppDbContext context)
        {
            _schoolClassRepository = schoolClassRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ClassDTO>> CreateClass([FromBody] CreateClassRequest request)
        {
            var existing = await _schoolClassRepository.GetByNameAsync(request.ClassName);
            if (existing != null)
            {
                return Conflict($"Przedmiot o nazwie '{request.ClassName}' już istnieje.");
            }

            var sc = new SchoolClass { ClassName = request.ClassName };
            await _schoolClassRepository.AddAsync(sc);
            return Ok(new ClassDTO(sc.Id, sc.ClassName));
        }


        [HttpGet]
        public async Task<ActionResult<List<ClassDTO>>> GetAllClasses()
        {
            var all = await _schoolClassRepository.GetAllAsync();
            var result = all.Select(sc => new ClassDTO(sc.Id, sc.ClassName)).ToList();
            return Ok(result);
        }

        [HttpGet("{classId}/teachers")]
        public async Task<ActionResult<List<ClassTeacherDTO>>> GetTeachersForClass(int classId)
        {
            try
            {
                Console.WriteLine($"🔍 Szukam nauczycieli dla przedmiotu (classId): {classId}");

                var rawData = await _context.GroupMemberClasses
                    .Include(gmc => gmc.GroupMember)
                        .ThenInclude(gm => gm.User)
                    .Include(gmc => gmc.GroupMember)
                        .ThenInclude(gm => gm.UserGroup)
                    .Where(gmc => gmc.SchoolClassId == classId)
                    .ToListAsync();

                foreach (var gmc in rawData)
                {
                    var gm = gmc.GroupMember;
                    var user = gm?.User;
                    var group = gm?.UserGroup;

                    Console.WriteLine($"➡️ GMC ID: {gmc.Id}, GroupMember ID: {gm?.Id}, " +
                        $"User: {user?.Name} {user?.Surname}, Role: {user?.UserRole}, Group: {group?.GroupName}");
                }

                var teachers = rawData
                    .Where(gmc => gmc.GroupMember != null &&
                                  gmc.GroupMember.User != null &&
                                  gmc.GroupMember.User.UserRole == AppUser.Role.T)
                    .Select(gmc => new ClassTeacherDTO
                    {
                        TeacherId = gmc.GroupMember.User.Id,
                        TeacherName = gmc.GroupMember.User.Name + " " + gmc.GroupMember.User.Surname,
                        GroupId = gmc.GroupMember.UserGroup.Id,
                        GroupName = gmc.GroupMember.UserGroup.GroupName
                    })
                    .ToList();

                Console.WriteLine($"✅ Znaleziono {teachers.Count} nauczycieli dla przedmiotu ID {classId}");

                return Ok(teachers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Błąd w GetTeachersForClass: " + ex.Message);
                return StatusCode(500, "Wewnętrzny błąd serwera");
            }
        }
    }
}
