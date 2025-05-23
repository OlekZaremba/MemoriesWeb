using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/classes")]
    public class SchoolClassController : ControllerBase
    {
        private readonly SchoolClassRepository _schoolClassRepository;

        public SchoolClassController(SchoolClassRepository schoolClassRepository)
        {
            _schoolClassRepository = schoolClassRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ClassDTO>> CreateClass([FromBody] CreateClassRequest request)
        {
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
    }
}
