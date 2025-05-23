using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoriesBack.DTO;
using MemoriesBack.DTOs;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _scheduleRepository;
        private readonly GroupMemberClassRepository _groupMemberClassRepository;

        public ScheduleService(
            ScheduleRepository scheduleRepository,
            GroupMemberClassRepository groupMemberClassRepository)
        {
            _scheduleRepository = scheduleRepository;
            _groupMemberClassRepository = groupMemberClassRepository;
        }

        public async Task<ScheduleResponseDTO> CreateLessonAsync(ScheduleRequestDTO dto)
        {
            var gmc = await _groupMemberClassRepository.GetByIdAsync(dto.AssignmentId)
                ?? throw new ArgumentException("Assignment not found");

            var first = new Schedule
            {
                GroupMemberClass = gmc,
                LessonDate = dto.LessonDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Generated = false
            };

            await _scheduleRepository.AddAsync(first);

            for (int i = 1; i <= 4; i++)
            {
                var nextDate = dto.LessonDate.AddDays(7 * i);
                if (nextDate.Month == dto.LessonDate.Month)
                {
                    var nextSchedule = new Schedule
                    {
                        GroupMemberClass = gmc,
                        LessonDate = nextDate,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        Generated = true
                    };

                    await _scheduleRepository.AddAsync(nextSchedule);
                }
            }

            var user = gmc.GroupMember.User;
            var schoolClass = gmc.SchoolClass;
            var userGroup = gmc.GroupMember.UserGroup;

            return new ScheduleResponseDTO(
                first.Id,
                gmc.Id,
                first.LessonDate,
                first.StartTime,
                first.EndTime,
                $"{user.Name} {user.Surname}",
                schoolClass.ClassName,
                userGroup.GroupName
            );
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleForGroupAsync(int groupId, DateTime from, DateTime to)
        {
            return await _scheduleRepository.GetByGroupAndDateRangeAsync(groupId, from, to);
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleForTeacherAsync(int teacherId, DateTime from, DateTime to)
        {
            return await _scheduleRepository.GetByTeacherAndDateRangeAsync(teacherId, from, to);
        }
    }
}
