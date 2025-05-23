using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class AssignmentService
    {
        private readonly UserRepository _userRepo;
        private readonly UserGroupRepository _groupRepo;
        private readonly GroupMemberRepository _groupMemberRepo;
        private readonly SchoolClassRepository _classRepo;
        private readonly GroupMemberClassRepository _gmClassRepo;

        public AssignmentService(
            UserRepository userRepo,
            UserGroupRepository groupRepo,
            GroupMemberRepository groupMemberRepo,
            SchoolClassRepository classRepo,
            GroupMemberClassRepository gmClassRepo)
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

            var member = new GroupMember { User = teacher, UserId = teacherId, UserGroup = group, UserGroupId = groupId };
            await _groupMemberRepo.AddAsync(member);
        }

        public async Task AssignTeacherToClassAsync(int teacherId, int groupId, int classId)
        {
            var groupMembers = await _groupMemberRepo.GetByUserGroupIdAsync(groupId);
            var gm = groupMembers.FirstOrDefault(g => g.UserId == teacherId)
                ?? throw new ArgumentException("Nauczyciel nie należy do tej grupy");

            var subject = await _classRepo.GetByIdAsync(classId)
                ?? throw new ArgumentException("Nie znaleziono przedmiotu");

            var link = new GroupMemberClass { GroupMember = gm, GroupMemberId = gm.Id, SchoolClass = subject, SchoolClassId = subject.Id };
            await _gmClassRepo.AddAsync(link);
        }

        public async Task<List<SchoolClass>> GetAssignedClassesAsync(int teacherId, int groupId)
        {
            var groupMembers = await _groupMemberRepo.GetByUserGroupIdAsync(groupId);
            var gm = groupMembers.FirstOrDefault(m => m.UserId == teacherId)
                ?? throw new ArgumentException("Brak przypisanego nauczyciela");

            var assignments = await _gmClassRepo.GetAllByGroupMemberIdAsync(gm.Id);
            return assignments.Select(a => a.SchoolClass).ToList();
        }
    }
}
