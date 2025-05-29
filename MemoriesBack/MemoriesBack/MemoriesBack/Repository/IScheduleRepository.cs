// Plik: IScheduleRepository.cs
// Lokalizacja: np. MemoriesBack/Repository/IScheduleRepository.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemoriesBack.Entities;
using MemoriesBack.DTOs; // Dla ScheduleResponseDTO

namespace MemoriesBack.Repository
{
    public interface IScheduleRepository
    {
        Task AddAsync(Schedule schedule);
        Task AddRangeAsync(IEnumerable<Schedule> schedules);
        Task<List<ScheduleResponseDTO>> GetByGroupAndDateRangeAsync(int groupId, DateTime from, DateTime to);
        Task<List<ScheduleResponseDTO>> GetByTeacherAndDateRangeAsync(int teacherId, DateTime from, DateTime to);
        Task<List<Schedule>> GetWithDetailsInRangeAsync(DateTime from, DateTime to);
    }
}