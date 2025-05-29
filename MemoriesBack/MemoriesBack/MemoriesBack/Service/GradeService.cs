using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Upewnij się, że ten using jest obecny
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
                StudentId = student.Id, 
                TeacherId = teacher.Id, 
                SchoolClassId = schoolClass.Id, 
                IssueDate = DateTime.Today,
                Notified = false
            };

            await _gradeRepository.AddAsync(grade);
        }

        // --- ZMODYFIKOWANA METODA ---
        public async Task<List<SchoolClassDTO>> GetSubjectsForStudentAsync(int userId)
        {
            // 1. Pobierz wszystkie oceny dla danego ucznia.
            // WAŻNE: Upewnij się, że metoda GetByStudentIdAsync w Twoim GradeRepository
            // dołącza powiązaną encję SchoolClass. Powinno to wyglądać mniej więcej tak:
            //
            // public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
            // {
            //     return await _context.Grades
            //         .Include(g => g.SchoolClass) // To jest kluczowe!
            //         .Where(g => g.StudentId == studentId)
            //         .ToListAsync();
            // }
            var studentGrades = await _gradeRepository.GetByStudentIdAsync(userId);

            if (studentGrades == null || !studentGrades.Any())
            {
                // Jeśli uczeń nie ma żadnych ocen, można zwrócić pustą listę.
                // Alternatywnie, można by pobrać przedmioty, do których jest formalnie przypisany
                // (np. przez tabelę group_members_has_class), nawet jeśli nie ma z nich ocen.
                // Dla uproszczenia, jeśli nie ma ocen, zakładamy, że nie ma "aktywnych" przedmiotów do wyświetlenia.
                return new List<SchoolClassDTO>();
            }

            // 2. Wyodrębnij unikalne przedmioty (SchoolClass) z listy ocen.
            // Używamy GroupBy po SchoolClassId, a następnie bierzemy pierwszy SchoolClass z każdej grupy,
            // aby mieć pewność, że mamy unikalne obiekty SchoolClass.
            var distinctSchoolClasses = studentGrades
                .Where(g => g.SchoolClass != null) // Upewnij się, że SchoolClass nie jest null (powinno być załadowane przez Include)
                .GroupBy(g => g.SchoolClassId)
                .Select(group => group.First().SchoolClass)
                .ToList();

            var result = new List<SchoolClassDTO>();

            // 3. Dla każdego unikalnego przedmiotu oblicz średnią ocen ucznia z tego przedmiotu.
            foreach (var sc in distinctSchoolClasses)
            {
                // Filtruj oceny tylko dla bieżącego przedmiotu, aby poprawnie obliczyć średnią.
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
        // --- KONIEC ZMODYFIKOWANEJ METODY ---

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

            foreach (var gradeEntity in grades) // Zmieniono nazwę zmiennej, aby uniknąć konfliktu
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
                var grades = await _gradeRepository.GetByTeacherAndClassAsync(teacherId, sc.Id);
                double avg = grades.Any() ? grades.Average(g => g.GradeValue) : 0.0;

                result.Add(new SchoolClassDTO(sc.Id, sc.ClassName, avg));
            }

            return result;
        }

        public async Task<List<GroupStudentWithGradesDTO>> GetStudentsForGroupAsync(int groupId)
        {
            var members = await _groupMemberRepository.GetByUserGroupIdWithUsersAsync(groupId);

            if (members == null || !members.Any()) // Poprawiono na Any()
                return new List<GroupStudentWithGradesDTO>();

            var result = new List<GroupStudentWithGradesDTO>();

            // Upewnij się, że User nie jest null przed próbą dostępu do UserRole
            foreach (var student in members.Where(m => m.User != null && m.User.UserRole == User.Role.S).Select(m => m.User))
            {
                var grades = await _gradeRepository.GetByStudentIdAsync(student.Id);

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
                    // Upewnij się, że g.Value jest interpretowane jako double
                    average = Math.Round(gradeDtos.Average(g => Convert.ToDouble(g.Value)), 2);
                }

                result.Add(new GroupStudentWithGradesDTO(
                    student.Id,
                    student.Name,
                    student.Surname,
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
