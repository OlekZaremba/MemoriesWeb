// Plik: ScheduleService.cs
// Lokalizacja: MemoriesBack/Service/ScheduleService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoriesBack.DTO;
using MemoriesBack.DTOs;
using MemoriesBack.Entities;
using MemoriesBack.Repository; // Upewnij się, że masz using do przestrzeni nazw, gdzie są interfejsy

namespace MemoriesBack.Service
{
    public class ScheduleService
    {
        // ZMIANA: Typy pól zmienione na interfejsy
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IGroupMemberClassRepository _groupMemberClassRepository;

        // ZMIANA: Typy parametrów w konstruktorze zmienione na interfejsy
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

            // Wywołanie metody z interfejsu IGroupMemberClassRepository
            var gmc = await _groupMemberClassRepository.GetByIdAsync(dto.AssignmentId);

            if (gmc == null)
            {
                Console.WriteLine("❌ GroupMemberClass not found for assignmentId = " + dto.AssignmentId);
                throw new ArgumentException("Assignment not found");
            }

            // Sprawdzenie, czy SchoolClass jest załadowane (powinno być dzięki Include w repozytorium)
            if (gmc.SchoolClass == null)
            {
                // To nie powinno się zdarzyć, jeśli GetByIdAsync w repozytorium działa poprawnie
                Console.WriteLine("❌ SchoolClass not loaded for GroupMemberClass ID = " + gmc.Id);
                throw new InvalidOperationException("Critical error: SchoolClass details not loaded for the assignment.");
            }
            if (gmc.GroupMember == null || gmc.GroupMember.User == null || gmc.GroupMember.UserGroup == null)
            {
                // To również nie powinno się zdarzyć, jeśli GetByIdAsync w repozytorium działa poprawnie
                 Console.WriteLine("❌ GroupMember, User or UserGroup not loaded for GroupMemberClass ID = " + gmc.Id);
                throw new InvalidOperationException("Critical error: GroupMember details not loaded for the assignment.");
            }


            Console.WriteLine("✅ GroupMemberClass found:");
            Console.WriteLine($"   ID = {gmc.Id}");
            Console.WriteLine($"   GroupMemberId = {gmc.GroupMember?.Id}"); // Użycie ?. dla bezpieczeństwa, choć wyżej sprawdzamy
            Console.WriteLine($"   User = {gmc.GroupMember?.User?.Name} {gmc.GroupMember?.User?.Surname}");
            Console.WriteLine($"   Group = {gmc.GroupMember?.UserGroup?.GroupName}");
            Console.WriteLine($"   Class (Subject) = {gmc.SchoolClass?.ClassName}");


            var first = new Schedule
            {
                // GroupMemberClass = gmc, // Nie musimy już przypisywać całego obiektu, jeśli mamy ID
                GroupMemberClassId = gmc.Id, // Używamy ID bezpośrednio
                LessonDate = dto.LessonDate,
                StartTime = start,
                EndTime = end,
                Generated = false
            };

            // Wywołanie metody z interfejsu IScheduleRepository
            await _scheduleRepository.AddAsync(first);

            for (int i = 1; i <= 4; i++)
            {
                var nextDate = dto.LessonDate.AddDays(7 * i);
                if (nextDate.Month == dto.LessonDate.Month)
                {
                    var nextSchedule = new Schedule
                    {
                        // GroupMemberClass = gmc,
                        GroupMemberClassId = gmc.Id,
                        LessonDate = nextDate,
                        StartTime = start,
                        EndTime = end,
                        Generated = true
                    };
                    // Wywołanie metody z interfejsu IScheduleRepository
                    await _scheduleRepository.AddAsync(nextSchedule);
                }
            }
            // Aby MapToDto działało, potrzebuje obiektu 'first' z załadowanymi właściwościami nawigacyjnymi.
            // Jeśli 'first' nie ma ich (bo np. przypisaliśmy tylko GroupMemberClassId), MapToDto może się nie powieść.
            // _scheduleRepository.AddAsync nie zwraca zaktualizowanej encji z załadowanymi właściwościami.
            // Potrzebujemy obiektu Schedule 'first' z GroupMemberClass, który ma załadowane GroupMember, User, UserGroup, SchoolClass.
            // Obiekt 'gmc' już te dane posiada z _groupMemberClassRepository.GetByIdAsync.
            // Musimy przekazać 'gmc' do MapToDto lub zmodyfikować MapToDto,
            // albo, co lepsze, przekazać potrzebne dane do MapToDto.

            // Bezpieczniejsze podejście do MapToDto: przekazać 'gmc'
            // Lub zmodyfikować MapToDto, aby przyjmowało 'first' i 'gmc'
            // Najprościej: upewnić się, że MapToDto może działać na 'first' które ma 'gmc' przypisane
            first.GroupMemberClass = gmc; // Upewniamy się, że nawigacja jest ustawiona dla MapToDto

            return MapToDto(first);
        }


        public async Task<List<ScheduleResponseDTO>> GetScheduleForGroupAsync(int groupId, DateTime from, DateTime to)
        {
            // Wywołanie metody z interfejsu IScheduleRepository
            return await _scheduleRepository.GetByGroupAndDateRangeAsync(groupId, from, to);
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleForTeacherAsync(int teacherId, DateTime from, DateTime to)
        {
            // Wywołanie metody z interfejsu IScheduleRepository
            return await _scheduleRepository.GetByTeacherAndDateRangeAsync(teacherId, from, to);
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleInDateRangeAsync(DateTime from, DateTime to)
        {
            // Wywołanie metody z interfejsu IScheduleRepository
            var schedules = await _scheduleRepository.GetWithDetailsInRangeAsync(from, to);
            // Metoda GetWithDetailsInRangeAsync zwraca List<Schedule>
            // Upewnijmy się, że te encje 'Schedule' mają załadowane potrzebne nawigacje dla MapToDto
            // (Repozytorium GetWithDetailsInRangeAsync powinno to zapewniać przez .Include)
            return schedules.Select(MapToDto).ToList();
        }

        private ScheduleResponseDTO MapToDto(Schedule s)
        {
            // WAŻNE: Ta metoda zakłada, że s.GroupMemberClass oraz jego wewnętrzne właściwości
            // (GroupMember, User, SchoolClass, UserGroup) są załadowane.
            if (s.GroupMemberClass == null || 
                s.GroupMemberClass.GroupMember == null ||
                s.GroupMemberClass.GroupMember.User == null ||
                s.GroupMemberClass.GroupMember.UserGroup == null ||
                s.GroupMemberClass.SchoolClass == null)
            {
                // To jest sytuacja awaryjna, dane nie zostały poprawnie załadowane
                // przed wywołaniem MapToDto.
                throw new InvalidOperationException($"Niekompletne dane dla Schedule ID: {s.Id}. Wymagane właściwości nawigacyjne nie są załadowane.");
            }

            var user = s.GroupMemberClass.GroupMember.User;
            var schoolClass = s.GroupMemberClass.SchoolClass;
            var userGroup = s.GroupMemberClass.GroupMember.UserGroup;

            return new ScheduleResponseDTO(
                s.Id,
                s.GroupMemberClass.Id, // lub s.GroupMemberClassId, jeśli to jest to, czego potrzebujesz
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
