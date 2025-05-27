using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class GradeService
    {
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly GroupMemberClassRepository _groupMemberClassRepository;
        private readonly GradeRepository _gradeRepository;
        private readonly UserRepository _userRepository;
        private readonly SchoolClassRepository _schoolClassRepository;
        private readonly UserGroupRepository _userGroupRepository;

        public GradeService(
            GroupMemberRepository groupMemberRepository,
            GroupMemberClassRepository groupMemberClassRepository,
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
                Student = student,
                Teacher = teacher,
                SchoolClass = schoolClass,
                IssueDate = DateTime.Today,
                Notified = false
            };

            await _gradeRepository.AddAsync(grade);
        }

        public async Task<List<SchoolClassDTO>> GetSubjectsForStudentAsync(int userId)
        {
            var groupMember = await _groupMemberRepository.GetByUserIdAsync(userId)
                ?? throw new ArgumentException("Brak członkostwa w grupie");

            var groupId = groupMember.UserGroupId;
            var assignments = await _groupMemberClassRepository.GetAllByGroupMemberIdAsync(groupMember.Id);

            var classList = assignments
                .Select(gmc => gmc.SchoolClass)
                .Distinct()
                .ToList();

            var result = new List<SchoolClassDTO>();

            foreach (var sc in classList)
            {
                var grades = await _gradeRepository.GetByStudentAndClassAsync(userId, sc.Id);
                double average = grades.Any()
                    ? grades.Average(g => g.GradeValue)
                    : 0.0;

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
                    g.SchoolClass?.ClassName ?? ""
                ))
                .ToList();
        }

        public async Task<GradeDetailDTO> GetGradeDetailsAsync(int gradeId)
        {
            var g = await _gradeRepository.GetByIdAsync(gradeId)
                ?? throw new ArgumentException("Ocena nie istnieje");

            return new GradeDetailDTO(
                g.Id,
                g.GradeValue,
                g.Type ?? "",
                g.IssueDate.ToString("yyyy-MM-dd"),
                g.Description ?? "",
                $"{g.Student.Name} {g.Student.Surname}",
                $"{g.Teacher.Name} {g.Teacher.Surname}",
                g.SchoolClass?.ClassName ?? ""
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

            foreach (var g in grades)
            {
                g.Notified = true;
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
                var grades = await _gradeRepository.GetByTeacherAndClassAsync(teacherId, sc.Id);
                double avg = grades.Any() ? grades.Average(g => g.GradeValue) : 0.0;

                result.Add(new SchoolClassDTO(sc.Id, sc.ClassName, avg));
            }

            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsForGroupAsync(int groupId)
        {
            var members = await _groupMemberRepository.GetByUserGroupIdAsync(groupId);

            return members
                .Select(m => m.User)
                .Where(u => u.UserRole == User.Role.S)
                .Select(u => new StudentDTO(u.Id, u.Name, u.Surname))
                .ToList();
        }

        public async Task<List<GradeSummaryDTO>> GetAllGradesForStudentAsync(int studentId)
        {
            var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
            return grades.Select(g => new GradeSummaryDTO(
                g.Id,
                g.GradeValue,
                g.Type ?? "",
                g.IssueDate.ToString("yyyy-MM-dd"),
                g.SchoolClass?.ClassName ?? ""
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
