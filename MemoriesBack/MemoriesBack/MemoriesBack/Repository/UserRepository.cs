using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        
        public async Task<SensitiveData?> GetByEmailAsync(string email)
        {
            return await _context.SensitiveData
                .Include(sd => sd.User)
                .FirstOrDefaultAsync(sd => sd.Email == email);
        }
        
        public async Task<List<User>> GetByRoleAsync(User.Role role)
        {
            return await _context.Users
                .Where(u => u.UserRole == role)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
