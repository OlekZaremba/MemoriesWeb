using System;
using System.Threading.Tasks;
using MemoriesBack.DTO;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class GroupMemberClassService
    {
        private readonly GroupMemberClassRepository _repository;

        public GroupMemberClassService(GroupMemberClassRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClassDTO> FindSubjectByGroupAndTeacherAsync(int groupId, int teacherId)
        {
            var gmc = await _repository.GetFirstByGroupIdAndUserIdAsync(groupId, teacherId);
            if (gmc == null)
                throw new ArgumentException("Brak przypisania klasy/przedmiotu");

            var schoolClass = gmc.SchoolClass;
            return new ClassDTO(schoolClass.Id, schoolClass.ClassName);
        }
        public async Task<List<AssignmentDTO>> GetAssignmentsForGroup(int groupId)
        {
            var gmcList = await _repository.GetByUserGroupIdAsync(groupId);

            Console.WriteLine($"🔍 Dla grupy {groupId} znaleziono {gmcList.Count} przypisań");

            foreach (var gmc in gmcList)
            {
                Console.WriteLine($"➡️ GMC ID: {gmc.Id}, User: {gmc.GroupMember?.User?.Name} {gmc.GroupMember?.User?.Surname}, Rola: {gmc.GroupMember?.User?.UserRole}, Przedmiot: {gmc.SchoolClass?.ClassName}");
            }

            return gmcList
                .Where(gmc =>
                    gmc.GroupMember != null &&
                    gmc.GroupMember.User != null &&
                    gmc.SchoolClass != null &&
                    gmc.GroupMember.User.UserRole == User.Role.T)
                .Select(gmc => new AssignmentDTO(
                    gmc.Id,
                    $"{gmc.GroupMember.User.Name} {gmc.GroupMember.User.Surname}",
                    gmc.SchoolClass.ClassName,
                    gmc.SchoolClass.Id,
                    gmc.SchoolClass.ClassName
                ))
                .ToList();
        }
    }
}
