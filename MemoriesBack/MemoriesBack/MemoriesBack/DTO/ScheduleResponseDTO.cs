using System;

namespace MemoriesBack.DTOs
{
    public class ScheduleResponseDTO
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public DateTime LessonDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public string GroupName { get; set; }

        public ScheduleResponseDTO(int id, int assignmentId, DateTime lessonDate,
            TimeSpan startTime, TimeSpan endTime,
            string teacherName, string subjectName, string groupName)
        {
            Id = id;
            AssignmentId = assignmentId;
            LessonDate = lessonDate;
            StartTime = startTime;
            EndTime = endTime;
            TeacherName = teacherName;
            SubjectName = subjectName;
            GroupName = groupName;
        }
    }
}
