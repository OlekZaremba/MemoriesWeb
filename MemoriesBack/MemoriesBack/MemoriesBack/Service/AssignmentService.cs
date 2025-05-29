// Plik: AssignmentService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoriesBack.Entities;
using MemoriesBack.Repository; 
using MemoriesBack.DTO;

namespace MemoriesBack.Service
{
    public class AssignmentService
    {
        private readonly UserRepository _userRepo;
        private readonly UserGroupRepository _groupRepo;
        private readonly GroupMemberRepository _groupMemberRepo;
        private readonly SchoolClassRepository _classRepo;
        private readonly IGroupMemberClassRepository _gmClassRepo;
        

        public AssignmentService(
            UserRepository userRepo,
            UserGroupRepository groupRepo,
            GroupMemberRepository groupMemberRepo,
            SchoolClassRepository classRepo,
            IGroupMemberClassRepository gmClassRepo)
        {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _groupMemberRepo = groupMemberRepo;
            _classRepo = classRepo;
            _gmClassRepo = gmClassRepo;
        }

        public async Task AssignTeacherToGroupAsync(int teacherId, int groupId)
        {
            var teacher = await _userRepo.GetByIdAsync(teacherId)
                          ?? throw new ArgumentException("Nie znaleziono nauczyciela");
            var group = await _groupRepo.GetByIdAsync(groupId)
                        ?? throw new ArgumentException("Nie znaleziono grupy");
            
            var existing = await _groupMemberRepo.GetByUserIdAndGroupIdAsync(teacherId, groupId);
            if (existing != null)
            {
                throw new InvalidOperationException("Nauczyciel już przypisany do tej klasy.");
            }

            var member = new GroupMember
            {
                UserId = teacherId,
                UserGroupId = groupId
            };

            await _groupMemberRepo.AddAsync(member);
        }

        public async Task AssignTeacherToClassAsync(int teacherId, int groupId, int classId)
        {
            var gm = await _groupMemberRepo.GetByUserIdAndGroupIdAsync(teacherId, groupId)
                     ?? throw new ArgumentException($"Nauczyciel (ID: {teacherId}) nie należy do grupy (ID: {groupId}) lub nie znaleziono wpisu GroupMember.");

            var subject = await _classRepo.GetByIdAsync(classId)
                          ?? throw new ArgumentException("Nie znaleziono przedmiotu");
            
            var existingLinks = await _gmClassRepo.GetAllByGroupMemberIdAsync(gm.Id);
            if (existingLinks.Any(l => l.SchoolClassId == subject.Id))
            {
                throw new InvalidOperationException("To przypisanie (nauczyciel-przedmiot w tej grupie) już istnieje.");
            }

            var link = new GroupMemberClass
            {
                GroupMemberId = gm.Id,
                SchoolClassId = subject.Id
            };
            await _gmClassRepo.AddAsync(link);
        }


        public async Task<List<SchoolClass>> GetAssignedClassesAsync(int teacherId, int groupId)
        {
            var gm = await _groupMemberRepo.GetByUserIdAndGroupIdAsync(teacherId, groupId)
                ?? throw new ArgumentException($"Nauczyciel (ID: {teacherId}) nie jest przypisany do grupy (ID: {groupId}).");

            var assignments = await _gmClassRepo.GetAllByGroupMemberIdAsync(gm.Id);
            return assignments.Select(a => a.SchoolClass).Where(sc => sc != null).ToList()!;
        }
        
        public async Task<List<AssignmentDTO>> GetAllAssignmentsAsync()
        {
            // Teraz ta metoda powinna być rozpoznawana przez interfejs
            var assignments = await _gmClassRepo.GetAllAssignmentsWithClassAndTeacher(); 

            return assignments
                .Where(gmc => gmc.GroupMember?.User != null && gmc.SchoolClass != null) 
                .Select(gmc => new AssignmentDTO(
                    assignmentId: gmc.Id, 
                    teacherName: $"{gmc.GroupMember!.User!.Name} {gmc.GroupMember.User.Surname}",
                    subjectName: gmc.SchoolClass!.ClassName,
                    classId: gmc.SchoolClass.Id, 
                    className: gmc.SchoolClass.ClassName 
                ))
                .ToList();
        }
    }
}
