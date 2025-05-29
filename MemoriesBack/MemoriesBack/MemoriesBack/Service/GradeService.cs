// Plik: GradeService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository; // Upewnij się, że ta przestrzeń nazw zawiera interfejsy

namespace MemoriesBack.Service
{
    public class GradeService
    {
        private readonly GroupMemberRepository _groupMemberRepository; 
        // ZMIANA: Użycie interfejsu
        private readonly IGroupMemberClassRepository _groupMemberClassRepository;
        private readonly GradeRepository _gradeRepository; 
        private readonly UserRepository _userRepository; 
        private readonly SchoolClassRepository _schoolClassRepository; 
        private readonly UserGroupRepository _userGroupRepository;

        public GradeService(
            GroupMemberRepository groupMemberRepository,
            // ZMIANA: Typ parametru na interfejs
            IGroupMemberClassRepository groupMemberClassRepository,
            GradeRepository gradeRepository,
            UserRepository userRepository,
            SchoolClassRepository schoolClassRepository,
            UserGroupRepository userGroupRepository)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupMemberClassRepository = groupMemberClassRepository;
            _gradeRepository = gradeRepository;
            _userRepository = userRepository;
            _schoolClassRepository = schoolClassRepository;
            _userGroupRepository = userGroupRepository;
        }

        public async Task AddGradeAsync(GradeRequest req)
        {
            var student = await _userRepository.GetByIdAsync(req.StudentId)
                ?? throw new ArgumentException("Uczeń nie istnieje");
            var teacher = await _userRepository.GetByIdAsync(req.TeacherId)
                ?? throw new ArgumentException("Nauczyciel nie istnieje");
            var schoolClass = await _schoolClassRepository.GetByIdAsync(req.ClassId)
                ?? throw new ArgumentException("Przedmiot nie istnieje");

            var grade = new Grade
            {
                GradeValue = req.Grade,
                Description = req.Description,
                Type = req.Type,
                StudentId = student.Id, 
                TeacherId = teacher.Id, 
                SchoolClassId = schoolClass.Id, 
                IssueDate = DateTime.Today,
                Notified = false
            };

            await _gradeRepository.AddAsync(grade);
        }

        public async Task<List<SchoolClassDTO>> GetSubjectsForStudentAsync(int userId)
        {
            var studentGrades = await _gradeRepository.GetByStudentIdAsync(userId);

            if (studentGrades == null || !studentGrades.Any())
            {
                return new List<SchoolClassDTO>();
            }

            var distinctSchoolClasses = studentGrades
                .Where(g => g.SchoolClass != null) 
                .GroupBy(g => g.SchoolClassId)
                .Select(group => group.First().SchoolClass)
                .ToList();

            var result = new List<SchoolClassDTO>();

            foreach (var sc in distinctSchoolClasses)
            {
                if (sc == null) continue; // Dodatkowe zabezpieczenie
                var gradesForThisSubject = studentGrades
                    .Where(g => g.SchoolClassId == sc.Id)
                    .ToList();
                
                double average = 0.0;
                if (gradesForThisSubject.Any())
                {
                    average = gradesForThisSubject.Average(g => g.GradeValue);
                }

                result.Add(new SchoolClassDTO(sc.Id, sc.ClassName, average));
            }

            return result;
        }

        public async Task<List<GradeSummaryDTO>> GetGradesForSubjectAsync(int studentId, int classId)
        {
            var grades = await _gradeRepository.GetByStudentAndClassAsync(studentId, classId);
            return grades
                .Select(g => new GradeSummaryDTO(
                    g.Id,
                    g.GradeValue,
                    g.Type ?? "",
                    g.IssueDate.ToString("yyyy-MM-dd"),
                    g.SchoolClass?.ClassName ?? "",
                    g.Description ?? ""
                ))
                .ToList();
        }

        public async Task<GradeDetailDTO> GetGradeDetailsAsync(int gradeId)
        {
            var g = await _gradeRepository.GetByIdAsync(gradeId)
                ?? throw new ArgumentException("Ocena nie istnieje");

            // Upewnij się, że GetByIdAsync w GradeRepository dołącza Student, Teacher, SchoolClass
            return new GradeDetailDTO(
                g.Id,
                g.GradeValue,
                g.Type ?? "",
                g.IssueDate.ToString("yyyy-MM-dd"),
                g.Description ?? "",
                (g.Student != null) ? $"{g.Student.Name} {g.Student.Surname}" : "N/A", 
                (g.Teacher != null) ? $"{g.Teacher.Name} {g.Teacher.Surname}" : "N/A", 
                g.SchoolClass?.ClassName ?? "N/A"
            );
        }

        public async Task<List<NewGradeDTO>> GetNewGradesForStudentAsync(int studentId)
        {
            var grades = await _gradeRepository.GetNotNotifiedGradesByStudentIdAsync(studentId);

            var dtos = grades.Select(g => new NewGradeDTO(
                g.Id,
                g.GradeValue,
                g.Type ?? "",
                g.IssueDate.ToString("yyyy-MM-dd"),
                g.SchoolClass?.ClassName ?? ""
            )).ToList();

            foreach (var gradeEntity in grades) 
            {
                gradeEntity.Notified = true;
            }

            await _gradeRepository.UpdateManyAsync(grades);

            return dtos;
        }

        public async Task<List<SchoolClassDTO>> GetClassesForTeacherAsync(int teacherId)
        {
            var classes = await _gradeRepository.GetDistinctClassesByTeacherIdAsync(teacherId);

            var result = new List<SchoolClassDTO>();
            foreach (var sc in classes)
            {
                if (sc == null) continue; // Dodatkowe zabezpieczenie
                var grades = await _gradeRepository.GetByTeacherAndClassAsync(teacherId, sc.Id);
                double avg = grades.Any() ? grades.Average(g => g.GradeValue) : 0.0;

                result.Add(new SchoolClassDTO(sc.Id, sc.ClassName, avg));
            }

            return result;
        }

        public async Task<List<GroupStudentWithGradesDTO>> GetStudentsForGroupAsync(int groupId)
        {
            var members = await _groupMemberRepository.GetByUserGroupIdWithUsersAsync(groupId);

            if (members == null || !members.Any()) 
                return new List<GroupStudentWithGradesDTO>();

            var result = new List<GroupStudentWithGradesDTO>();

            foreach (var studentUser in members.Where(m => m.User != null && m.User.UserRole == User.Role.S).Select(m => m.User))
            {
                if (studentUser == null) continue; // Dodatkowe zabezpieczenie
                var grades = await _gradeRepository.GetByStudentIdAsync(studentUser.Id);

                var gradeDtos = grades.Select(g => new GradeSimpleDTO(
                    g.Id,
                    g.GradeValue,
                    g.Type ?? "",
                    g.Description ?? "",
                    g.IssueDate.ToString("yyyy-MM-dd")
                )).ToList();

                double average = 0.0;
                if (gradeDtos.Any())
                {
                    average = Math.Round(gradeDtos.Average(g => Convert.ToDouble(g.Value)), 2);
                }

                result.Add(new GroupStudentWithGradesDTO(
                    studentUser.Id,
                    studentUser.Name,
                    studentUser.Surname,
                    gradeDtos,
                    average
                ));
            }

            return result;
        }

        public async Task<List<GradeSummaryDTO>> GetAllGradesForStudentAsync(int studentId)
        {
            var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
            return grades.Select(g => new GradeSummaryDTO(
                g.Id,
                g.GradeValue,
                g.Type ?? "",
                g.IssueDate.ToString("yyyy-MM-dd"),
                g.SchoolClass?.ClassName ?? "",
                g.Description ?? ""
            )).ToList();
        }

        public async Task<List<TeacherGroupDTO>> GetGroupsForTeacherAsync(int teacherId)
        {
            var groupIds = await _gradeRepository.GetDistinctGroupIdsByTeacherIdAsync(teacherId);
            var groups = await _userGroupRepository.GetByIdsAsync(groupIds);

            return groups.Select(g => new TeacherGroupDTO(g.Id, g.GroupName)).ToList();
        }
    }
}
