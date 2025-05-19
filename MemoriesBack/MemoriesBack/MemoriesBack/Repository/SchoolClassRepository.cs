using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class SchoolClassRepository
    {
        private readonly AppDbContext _context;

        public SchoolClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SchoolClass>> GetAllAsync()
        {
            return await _context.SchoolClasses.ToListAsync();
        }

        public async Task<SchoolClass?> GetByIdAsync(int id)
        {
            return await _context.SchoolClasses.FindAsync(id);
        }

        public async Task AddAsync(SchoolClass schoolClass)
        {
            await _context.SchoolClasses.AddAsync(schoolClass);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SchoolClass schoolClass)
        {
            _context.SchoolClasses.Update(schoolClass);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SchoolClass schoolClass)
        {
            _context.SchoolClasses.Remove(schoolClass);
            await _context.SaveChangesAsync();
        }
    }
}
