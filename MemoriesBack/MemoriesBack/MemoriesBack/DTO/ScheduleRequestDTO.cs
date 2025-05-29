namespace MemoriesBack.DTO
{
    public class ScheduleRequestDTO
    {
        public int AssignmentId { get; set; }
        public DateTime LessonDate { get; set; }
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }

}