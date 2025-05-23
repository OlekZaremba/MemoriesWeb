using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Entities;
using MemoriesBack.DTOs;

namespace MemoriesBack.Repository
{
    public class ScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Schedule> schedules)
        {
            await _context.Schedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduleResponseDTO>> GetByGroupAndDateRangeAsync(int groupId, DateTime from, DateTime to)
        {
            return await _context.Schedules
                .Include(s => s.GroupMemberClass)
                    .ThenInclude(gmc => gmc.GroupMember)
                        .ThenInclude(gm => gm.User)
                .Include(s => s.GroupMemberClass.SchoolClass)
                .Include(s => s.GroupMemberClass.GroupMember.UserGroup)
                .Where(s =>
                    s.GroupMemberClass.GroupMember.UserGroupId == groupId &&
                    s.LessonDate >= from && s.LessonDate <= to)
                .OrderBy(s => s.LessonDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new ScheduleResponseDTO(
                    s.Id,
                    s.GroupMemberClassId,
                    s.LessonDate,
                    s.StartTime,
                    s.EndTime,
                    s.GroupMemberClass.GroupMember.User.Name + " " + s.GroupMemberClass.GroupMember.User.Surname,
                    s.GroupMemberClass.SchoolClass.ClassName,
                    s.GroupMemberClass.GroupMember.UserGroup.GroupName
                ))
                .ToListAsync();
        }

        public async Task<List<ScheduleResponseDTO>> GetByTeacherAndDateRangeAsync(int teacherId, DateTime from, DateTime to)
        {
            return await _context.Schedules
                .Include(s => s.GroupMemberClass)
                    .ThenInclude(gmc => gmc.GroupMember)
                        .ThenInclude(gm => gm.User)
                .Include(s => s.GroupMemberClass.SchoolClass)
                .Include(s => s.GroupMemberClass.GroupMember.UserGroup)
                .Where(s =>
                    s.GroupMemberClass.GroupMember.User.Id == teacherId &&
                    s.LessonDate >= from && s.LessonDate <= to)
                .OrderBy(s => s.LessonDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new ScheduleResponseDTO(
                    s.Id,
                    s.GroupMemberClassId,
                    s.LessonDate,
                    s.StartTime,
                    s.EndTime,
                    s.GroupMemberClass.GroupMember.User.Name + " " + s.GroupMemberClass.GroupMember.User.Surname,
                    s.GroupMemberClass.SchoolClass.ClassName,
                    s.GroupMemberClass.GroupMember.UserGroup.GroupName
                ))
                .ToListAsync();
        }
    }
}
