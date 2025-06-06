﻿


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
        
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IGroupMemberClassRepository _groupMemberClassRepository;

        
        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IGroupMemberClassRepository groupMemberClassRepository)
        {
            _scheduleRepository = scheduleRepository;
            _groupMemberClassRepository = groupMemberClassRepository;
        }

        public async Task<ScheduleResponseDTO> CreateLessonAsync(ScheduleRequestDTO dto)
        {
            var start = TimeSpan.Parse(dto.StartTime);
            var end = TimeSpan.Parse(dto.EndTime);
    
            Console.WriteLine("▶ CreateLessonAsync called");
            Console.WriteLine($"assignmentId = {dto.AssignmentId}");
            Console.WriteLine($"lessonDate = {dto.LessonDate}");
            Console.WriteLine($"startTime = {dto.StartTime}");
            Console.WriteLine($"endTime = {dto.EndTime}");

            
            var gmc = await _groupMemberClassRepository.GetByIdAsync(dto.AssignmentId);

            if (gmc == null)
            {
                Console.WriteLine("❌ GroupMemberClass not found for assignmentId = " + dto.AssignmentId);
                throw new ArgumentException("Assignment not found");
            }

            
            if (gmc.SchoolClass == null)
            {
                
                Console.WriteLine("❌ SchoolClass not loaded for GroupMemberClass ID = " + gmc.Id);
                throw new InvalidOperationException("Critical error: SchoolClass details not loaded for the assignment.");
            }
            if (gmc.GroupMember == null || gmc.GroupMember.User == null || gmc.GroupMember.UserGroup == null)
            {
                
                 Console.WriteLine("❌ GroupMember, User or UserGroup not loaded for GroupMemberClass ID = " + gmc.Id);
                throw new InvalidOperationException("Critical error: GroupMember details not loaded for the assignment.");
            }


            Console.WriteLine("✅ GroupMemberClass found:");
            Console.WriteLine($"   ID = {gmc.Id}");
            Console.WriteLine($"   GroupMemberId = {gmc.GroupMember?.Id}"); 
            Console.WriteLine($"   User = {gmc.GroupMember?.User?.Name} {gmc.GroupMember?.User?.Surname}");
            Console.WriteLine($"   Group = {gmc.GroupMember?.UserGroup?.GroupName}");
            Console.WriteLine($"   Class (Subject) = {gmc.SchoolClass?.ClassName}");


            var first = new Schedule
            {
                
                GroupMemberClassId = gmc.Id, 
                LessonDate = dto.LessonDate,
                StartTime = start,
                EndTime = end,
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
                        
                        GroupMemberClassId = gmc.Id,
                        LessonDate = nextDate,
                        StartTime = start,
                        EndTime = end,
                        Generated = true
                    };
                    
                    await _scheduleRepository.AddAsync(nextSchedule);
                }
            }
            
            
            
            
            
            
            

            
            
            
            first.GroupMemberClass = gmc; 

            return MapToDto(first);
        }


        public async Task<List<ScheduleResponseDTO>> GetScheduleForGroupAsync(int groupId, DateTime from, DateTime to)
        {
            
            return await _scheduleRepository.GetByGroupAndDateRangeAsync(groupId, from, to);
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleForTeacherAsync(int teacherId, DateTime from, DateTime to)
        {
            
            return await _scheduleRepository.GetByTeacherAndDateRangeAsync(teacherId, from, to);
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleInDateRangeAsync(DateTime from, DateTime to)
        {
            
            var schedules = await _scheduleRepository.GetWithDetailsInRangeAsync(from, to);
            
            
            
            return schedules.Select(MapToDto).ToList();
        }

        private ScheduleResponseDTO MapToDto(Schedule s)
        {
            
            
            if (s.GroupMemberClass == null || 
                s.GroupMemberClass.GroupMember == null ||
                s.GroupMemberClass.GroupMember.User == null ||
                s.GroupMemberClass.GroupMember.UserGroup == null ||
                s.GroupMemberClass.SchoolClass == null)
            {
                
                
                throw new InvalidOperationException($"Niekompletne dane dla Schedule ID: {s.Id}. Wymagane właściwości nawigacyjne nie są załadowane.");
            }

            var user = s.GroupMemberClass.GroupMember.User;
            var schoolClass = s.GroupMemberClass.SchoolClass;
            var userGroup = s.GroupMemberClass.GroupMember.UserGroup;

            return new ScheduleResponseDTO(
                s.Id,
                s.GroupMemberClass.Id, 
                s.LessonDate,
                s.StartTime,
                s.EndTime,
                $"{user.Name} {user.Surname}",
                schoolClass.ClassName,
                userGroup.GroupName
            );
        }
    }
}
