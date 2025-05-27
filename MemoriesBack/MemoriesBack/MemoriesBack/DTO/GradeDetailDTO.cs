namespace MemoriesBack.DTO
{
    public class GradeDetailDTO
    {
        public int Id { get; set; }
        public double Grade { get; set; }
        public string Type { get; set; }
        public string IssueDate { get; set; }
        public string Description { get; set; }
        public string StudentName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }

        public GradeDetailDTO(int id, double grade, string type, string issueDate, string description, string studentName, string teacherName, string className)
        {
            Id = id;
            Grade = grade;
            Type = type;
            IssueDate = issueDate;
            Description = description;
            StudentName = studentName;
            TeacherName = teacherName;
            ClassName = className;
        }
    }
}