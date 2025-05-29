// Plik: GroupMemberClassServiceTests.cs
// Lokalizacja: W Twoim projekcie testowym, np. MemoriesBack.Tests/Service/GroupMemberClassServiceTests.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using MemoriesBack.Service;
using MemoriesBack.Repository;
using MemoriesBack.Entities;
using MemoriesBack.DTO; // Dla ClassDTO i AssignmentDTO

namespace MemoriesBack.Tests.Service
{
    public class GroupMemberClassServiceTests
    {
        private readonly Mock<IGroupMemberClassRepository> _mockRepository;
        private readonly GroupMemberClassService _service;

        public GroupMemberClassServiceTests()
        {
            _mockRepository = new Mock<IGroupMemberClassRepository>();
            _service = new GroupMemberClassService(_mockRepository.Object);
        }

        // Testy dla FindSubjectByGroupAndTeacherAsync
        [Fact]
        public async Task FindSubjectByGroupAndTeacherAsync_WhenAssignmentExistsAndSchoolClassIsValid_ReturnsClassDTO()
        {
            // Arrange
            var groupId = 1;
            var teacherId = 10;
            var expectedSchoolClass = new SchoolClass { Id = 100, ClassName = "Matematyka" };
            var fakeGmc = new GroupMemberClass
            {
                Id = 1,
                GroupMemberId = 1,
                SchoolClassId = expectedSchoolClass.Id,
                SchoolClass = expectedSchoolClass,
                GroupMember = new GroupMember { UserId = teacherId, UserGroupId = groupId }
            };

            _mockRepository.Setup(repo => repo.GetFirstByGroupIdAndUserIdAsync(groupId, teacherId))
                           .ReturnsAsync(fakeGmc);

            // Act
            var result = await _service.FindSubjectByGroupAndTeacherAsync(groupId, teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSchoolClass.Id, result.Id);
            Assert.Equal(expectedSchoolClass.ClassName, result.ClassName);
        }

        [Fact]
        public async Task FindSubjectByGroupAndTeacherAsync_WhenAssignmentNotFound_ThrowsArgumentException()
        {
            // Arrange
            var groupId = 1;
            var teacherId = 10;

            _mockRepository.Setup(repo => repo.GetFirstByGroupIdAndUserIdAsync(groupId, teacherId))
                           .ReturnsAsync((GroupMemberClass)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.FindSubjectByGroupAndTeacherAsync(groupId, teacherId)
            );
            Assert.Equal("Brak przypisania nauczyciela do przedmiotu w tej grupie.", exception.Message);
        }

        [Fact]
        public async Task FindSubjectByGroupAndTeacherAsync_WhenSchoolClassNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var groupId = 1;
            var teacherId = 10;
            var fakeGmcWithNullSchoolClass = new GroupMemberClass
            {
                Id = 1,
                GroupMemberId = 1,
                SchoolClass = null, // Symulacja braku załadowania SchoolClass
                GroupMember = new GroupMember { UserId = teacherId, UserGroupId = groupId }
            };

            _mockRepository.Setup(repo => repo.GetFirstByGroupIdAndUserIdAsync(groupId, teacherId))
                           .ReturnsAsync(fakeGmcWithNullSchoolClass);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.FindSubjectByGroupAndTeacherAsync(groupId, teacherId)
            );
            Assert.Equal("Nie udało się załadować danych przedmiotu dla znalezionego przypisania.", exception.Message);
        }

        // Testy dla GetAssignmentsForGroup
        [Fact]
        public async Task GetAssignmentsForGroup_WhenAssignmentsExistForTeachers_ReturnsMappedAssignmentDTOs()
        {
            // Arrange
            var groupId = 1;
            var teacherUser = new User { Id = 10, Name = "Jan", Surname = "Kowalski", UserRole = User.Role.T };
            var schoolClassMath = new SchoolClass { Id = 100, ClassName = "Matematyka" };
            var schoolClassPhysics = new SchoolClass { Id = 101, ClassName = "Fizyka" };

            var gmcList = new List<GroupMemberClass>
            {
                new GroupMemberClass 
                { 
                    Id = 1, 
                    GroupMember = new GroupMember { User = teacherUser, UserGroupId = groupId }, 
                    SchoolClass = schoolClassMath 
                },
                new GroupMemberClass 
                { 
                    Id = 2, 
                    GroupMember = new GroupMember { User = teacherUser, UserGroupId = groupId }, 
                    SchoolClass = schoolClassPhysics 
                },
                new GroupMemberClass // Przypisanie dla ucznia, powinno być odfiltrowane
                { 
                    Id = 3, 
                    GroupMember = new GroupMember { User = new User { Id = 20, UserRole = User.Role.S}, UserGroupId = groupId }, 
                    SchoolClass = schoolClassMath 
                },
                 new GroupMemberClass // Przypisanie dla nauczyciela, ale bez SchoolClass, powinno być odfiltrowane
                {
                    Id = 4,
                    GroupMember = new GroupMember { User = teacherUser, UserGroupId = groupId },
                    SchoolClass = null
                }
            };

            _mockRepository.Setup(repo => repo.GetByUserGroupIdAsync(groupId))
                           .ReturnsAsync(gmcList);

            // Act
            var result = await _service.GetAssignmentsForGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); 

            var mathAssignment = result.FirstOrDefault(a => a.SubjectName == "Matematyka");
            Assert.NotNull(mathAssignment);
            Assert.Equal(1, mathAssignment.AssignmentId);
            Assert.Equal("Jan Kowalski", mathAssignment.TeacherName);
            Assert.Equal(schoolClassMath.Id, mathAssignment.ClassId); // ClassId to ID przedmiotu

            var physicsAssignment = result.FirstOrDefault(a => a.SubjectName == "Fizyka");
            Assert.NotNull(physicsAssignment);
            Assert.Equal(2, physicsAssignment.AssignmentId);
        }

        [Fact]
        public async Task GetAssignmentsForGroup_WhenNoTeacherAssignmentsExist_ReturnsEmptyList()
        {
            // Arrange
            var groupId = 1;
            var studentUser = new User { Id = 20, Name = "Anna", Surname = "Nowak", UserRole = User.Role.S };
            var schoolClassMath = new SchoolClass { Id = 100, ClassName = "Matematyka" };
            
            var gmcListOnlyStudents = new List<GroupMemberClass>
            {
                new GroupMemberClass 
                { 
                    Id = 1, 
                    GroupMember = new GroupMember { User = studentUser, UserGroupId = groupId }, 
                    SchoolClass = schoolClassMath 
                }
            };

            _mockRepository.Setup(repo => repo.GetByUserGroupIdAsync(groupId))
                           .ReturnsAsync(gmcListOnlyStudents);

            // Act
            var result = await _service.GetAssignmentsForGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAssignmentsForGroup_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var groupId = 1;
            _mockRepository.Setup(repo => repo.GetByUserGroupIdAsync(groupId))
                           .ReturnsAsync(new List<GroupMemberClass>());

            // Act
            var result = await _service.GetAssignmentsForGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
         [Fact]
        public async Task GetAssignmentsForGroup_WhenGmcHasNullUserOrSchoolClass_FiltersOutAndReturnsCorrectAssignments()
        {
            // Arrange
            var groupId = 1;
            var teacherUser = new User { Id = 10, Name = "Jan", Surname = "Kowalski", UserRole = User.Role.T };
            var schoolClassMath = new SchoolClass { Id = 100, ClassName = "Matematyka" };

            var gmcList = new List<GroupMemberClass>
            {
                new GroupMemberClass // Poprawne przypisanie
                {
                    Id = 1,
                    GroupMember = new GroupMember { User = teacherUser, UserGroupId = groupId },
                    SchoolClass = schoolClassMath
                },
                new GroupMemberClass // Brak SchoolClass
                {
                    Id = 2,
                    GroupMember = new GroupMember { User = teacherUser, UserGroupId = groupId },
                    SchoolClass = null
                },
                new GroupMemberClass // Brak User w GroupMember
                {
                    Id = 3,
                    GroupMember = new GroupMember { User = null, UserGroupId = groupId },
                    SchoolClass = schoolClassMath
                },
                 new GroupMemberClass // Brak GroupMember
                {
                    Id = 4,
                    GroupMember = null,
                    SchoolClass = schoolClassMath
                }
            };

            _mockRepository.Setup(repo => repo.GetByUserGroupIdAsync(groupId))
                           .ReturnsAsync(gmcList);
            // Act
            var result = await _service.GetAssignmentsForGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Powinien zostać tylko jeden poprawny element
            Assert.Equal(1, result[0].AssignmentId);
            Assert.Equal("Matematyka", result[0].SubjectName);
        }
    }
}
