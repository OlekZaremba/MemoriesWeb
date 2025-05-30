﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;

namespace MemoriesBack.Repository
{
    public class GroupMemberRepository
    {
        private readonly AppDbContext _context;

        public GroupMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMember>> GetAllByUserGroupIdAsync(int userGroupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserGroupId == userGroupId)
                .ToListAsync();
        }

        public async Task<List<GroupMember>> GetAllByUserIdAsync(int userId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .ToListAsync();
        }

        public async Task<GroupMember?> GetByUserIdAsync(int userId)
        {
            return await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == userId);
        }

        public async Task<List<GroupMember>> GetByUserGroupIdAsync(int userGroupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserGroupId == userGroupId)
                .ToListAsync();
        }
        
        public async Task<List<GroupMember>> GetByUserGroupIdWithUsersAsync(int groupId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.UserGroupId == groupId)
                .ToListAsync();
        }
        
        public async Task AddAsync(GroupMember gm)
        {
            await _context.GroupMembers.AddAsync(gm);
            await _context.SaveChangesAsync();
        }
        
        public async Task<GroupMember?> GetByUserIdAndGroupIdAsync(int userId, int groupId)
        {
            return await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.UserGroupId == groupId);
        }
        
        public async Task<List<GroupMember>> GetTeachersWithGroupsAsync()
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Include(gm => gm.UserGroup)
                .Where(gm => gm.User.UserRole == User.Role.T)
                .ToListAsync();
        }

    }
}
