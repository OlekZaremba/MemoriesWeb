using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class UserGroupRepository
    {
        private readonly AppDbContext _context;

        public UserGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserGroup>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.UserGroups
                .Where(ug => ug.Members.Any(m => m.UserId == userId))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UserGroup>> GetAllAsync()
        {
            return await _context.UserGroups.ToListAsync();
        }

        public async Task<UserGroup?> GetByIdAsync(int id)
        {
            return await _context.UserGroups.FindAsync(id);
        }

        public async Task AddAsync(UserGroup group)
        {
            await _context.UserGroups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserGroup group)
        {
            _context.UserGroups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserGroup group)
        {
            _context.UserGroups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}
