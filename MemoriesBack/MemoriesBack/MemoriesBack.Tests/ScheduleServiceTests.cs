// Plik: ScheduleServiceTests.cs
// Lokalizacja: W Twoim projekcie testowym, np. MemoriesBack.Tests/ScheduleServiceTests.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit; // Lub using Microsoft.VisualStudio.TestTools.UnitTesting; dla MSTest
using Moq;
using MemoriesBack.Service;
using MemoriesBack.Repository;
using MemoriesBack.Entities;
using MemoriesBack.DTO;
using MemoriesBack.DTOs;

namespace MemoriesBack.Tests.Service
{
    public class ScheduleServiceTests
    {
        private readonly Mock<IScheduleRepository> _mockScheduleRepository;
        private readonly Mock<IGroupMemberClassRepository> _mockGroupMemberClassRepository;
        private readonly ScheduleService _scheduleService;

        public ScheduleServiceTests()
        {
            _mockScheduleRepository = new Mock<IScheduleRepository>();
            _mockGroupMemberClassRepository = new Mock<IGroupMemberClassRepository>();
            
            _scheduleService = new ScheduleService(
                _mockScheduleRepository.Object, 
                _mockGroupMemberClassRepository.Object
            );
        }

        [Fact]
        public async Task CreateLessonAsync_WhenAssignmentExists_CreatesLessonAndRecurringLessonsInSameMonth()
        {
            var assignmentId = 1;
            var lessonDate = new DateTime(2024, 6, 10);
            var startTime = "08:00";
            var endTime = "09:00";

            var requestDto = new ScheduleRequestDTO
            {
                AssignmentId = assignmentId,
                LessonDate = lessonDate,
                StartTime = startTime,
                EndTime = endTime
            };

            var fakeGmc = new GroupMemberClass
            {
                Id = assignmentId,
                GroupMemberId = 1,
                SchoolClassId = 1,
                SchoolClass = new SchoolClass { Id = 1, ClassName = "Test Subject" },
                GroupMember = new GroupMember 
                { 
                    Id = 1, 
                    UserGroupId = 1, 
                    UserId = 1,
                    User = new User { Id = 1, Name = "Test", Surname = "Teacher" },
                    UserGroup = new UserGroup { Id = 1, GroupName = "Test Group" }
                }
            };
            _mockGroupMemberClassRepository
                .Setup(repo => repo.GetByIdAsync(assignmentId))
                .ReturnsAsync(fakeGmc);                       

            // Lista do przechwycenia obiektów Schedule wysyłanych do AddAsync
            var addedSchedules = new List<Schedule>();
            _mockScheduleRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>())) 
                .Callback<Schedule>(s => addedSchedules.Add(s))     
                .Returns(Task.CompletedTask);                       

            // Act (Działanie)
            var resultDto = await _scheduleService.CreateLessonAsync(requestDto);

            // Assert (Sprawdzenie)
            // 1. Sprawdź, czy zwrócone DTO jest poprawne
            Assert.NotNull(resultDto);
            Assert.Equal(fakeGmc.SchoolClass.ClassName, resultDto.SubjectName);
            Assert.Equal(fakeGmc.GroupMember.UserGroup.GroupName, resultDto.GroupName);
            Assert.Equal($"{fakeGmc.GroupMember.User.Name} {fakeGmc.GroupMember.User.Surname}", resultDto.TeacherName);


            Assert.Equal(3, addedSchedules.Count); 
            _mockScheduleRepository.Verify(repo => repo.AddAsync(It.IsAny<Schedule>()), Times.Exactly(3));

            var firstLesson = addedSchedules.FirstOrDefault(s => !s.Generated);
            Assert.NotNull(firstLesson);
            Assert.Equal(lessonDate, firstLesson.LessonDate);
            Assert.Equal(TimeSpan.Parse(startTime), firstLesson.StartTime);
            Assert.Equal(fakeGmc.Id, firstLesson.GroupMemberClassId);

            var recurringLessons = addedSchedules.Where(s => s.Generated).ToList();
            Assert.Equal(2, recurringLessons.Count); // Powinny być 2 cykliczne w czerwcu

            Assert.Contains(recurringLessons, s => s.LessonDate == lessonDate.AddDays(7));
            Assert.Contains(recurringLessons, s => s.LessonDate == lessonDate.AddDays(14));
        }

        [Fact]
        public async Task CreateLessonAsync_WhenAssignmentNotFound_ThrowsArgumentException()
        {
            var assignmentId = 99; 
            var requestDto = new ScheduleRequestDTO
            {
                AssignmentId = assignmentId,
                LessonDate = DateTime.Now,
                StartTime = "10:00",
                EndTime = "11:00"
            };

            _mockGroupMemberClassRepository
                .Setup(repo => repo.GetByIdAsync(assignmentId))
                .ReturnsAsync((GroupMemberClass)null); 

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _scheduleService.CreateLessonAsync(requestDto)
            );
            Assert.Equal("Assignment not found", exception.Message); 
        }
    }
}
