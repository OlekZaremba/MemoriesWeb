using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class GroupMemberClassRepository
    {
        private readonly AppDbContext _context;

        public GroupMemberClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMemberClass>> GetAllByGroupMemberIdAsync(int groupMemberId)
        {
            return await _context.GroupMemberClasses
                .Include(gmc => gmc.SchoolClass)
                .Where(gmc => gmc.GroupMemberId == groupMemberId)
                .ToListAsync();
        }
        

        public async Task<List<GroupMemberClass>> GetByGroupIdAsync(int groupId)
        {
            return await _context.GroupMemberClasses
                .Where(gmc => gmc.GroupMember.UserGroupId == groupId)
                .ToListAsync();
        }

        public async Task<GroupMemberClass?> GetFirstByGroupIdAndUserIdAsync(int groupId, int userId)
        {
            return await _context.GroupMemberClasses
                .Where(gmc =>
                    gmc.GroupMember.UserGroupId == groupId &&
                    gmc.GroupMember.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GroupMemberClass>> GetByGroupMemberIdAsync(int groupMemberId)
        {
            return await _context.GroupMemberClasses
                .Where(gmc => gmc.GroupMemberId == groupMemberId)
                .ToListAsync();
        }

        public async Task<List<GroupMemberClass>> GetByClassIdAsync(int classId)
        {
            return await _context.GroupMemberClasses
                .Where(gmc => gmc.SchoolClassId == classId)
                .ToListAsync();
        }

        public async Task<List<GroupMemberClass>> GetByUserGroupIdAsync(int userGroupId)
        {
            return await _context.GroupMemberClasses
                .Include(gmc => gmc.SchoolClass)
                .Include(gmc => gmc.GroupMember) 
                .Where(gmc => gmc.GroupMember.UserGroupId == userGroupId)
                .ToListAsync();
        }

        public async Task<GroupMemberClass?> GetByIdAsync(int id)
        {
            return await _context.GroupMemberClasses
                .Include(gmc => gmc.GroupMember)
                    .ThenInclude(gm => gm.User)
                .Include(gmc => gmc.GroupMember.UserGroup)
                .Include(gmc => gmc.SchoolClass)
                .FirstOrDefaultAsync(gmc => gmc.Id == id);
        }
        
        public async Task AddAsync(GroupMemberClass entity)
        {
            await _context.GroupMemberClasses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
