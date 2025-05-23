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
    }
}
