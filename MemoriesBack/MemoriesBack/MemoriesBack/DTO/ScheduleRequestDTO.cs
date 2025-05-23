namespace MemoriesBack.DTO
{
    public class ScheduleRequestDTO
    {
        public int AssignmentId { get; set; }
        public DateTime LessonDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ScheduleRequestDTO() { }
    }
}