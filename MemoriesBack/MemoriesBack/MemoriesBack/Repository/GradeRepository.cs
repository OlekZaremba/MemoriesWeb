using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class GradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grade>> GetByStudentAndClassAsync(int studentId, int classId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId && g.SchoolClassId == classId)
                .OrderByDescending(g => g.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetNotNotifiedByStudentAsync(int studentId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId && !g.Notified)
                .ToListAsync();
        }

        public async Task<IEnumerable<SchoolClass>> GetDistinctClassesByTeacherAsync(int teacherId)
        {
            return await _context.Grades
                .Where(g => g.TeacherId == teacherId)
                .Select(g => g.SchoolClass)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByTeacherAndClassAsync(int teacherId, int classId)
        {
            return await _context.Grades
                .Where(g => g.TeacherId == teacherId && g.SchoolClassId == classId)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetDistinctGroupIdsByTeacherAsync(int teacherId)
        {
            return await _context.GroupMembers
                .Where(gm =>
                    _context.Grades.Any(g =>
                        g.TeacherId == teacherId &&
                        g.StudentId == gm.UserId))
                .Select(gm => gm.UserGroupId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByStudentOrderedDescAsync(int studentId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.Id)
                .ToListAsync();
        }
    }
}
