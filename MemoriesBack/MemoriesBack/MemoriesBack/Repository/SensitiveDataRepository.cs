using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class SensitiveDataRepository
    {
        private readonly AppDbContext _context;

        public SensitiveDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SensitiveData?> GetByLoginAsync(string login)
        {
            return await _context.SensitiveData
                .Include(s => s.User) 
                .FirstOrDefaultAsync(s => s.Login == login);
        }
        
        public async Task<SensitiveData?> GetByUserAsync(User user)
        {
            return await _context.SensitiveData
                .FirstOrDefaultAsync(sd => sd.UserId == user.Id);
        }

        public async Task AddAsync(SensitiveData data)
        {
            await _context.SensitiveData.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SensitiveData data)
        {
            _context.SensitiveData.Update(data);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SensitiveData data)
        {
            _context.SensitiveData.Remove(data);
            await _context.SaveChangesAsync();
        }
        
    }
}
