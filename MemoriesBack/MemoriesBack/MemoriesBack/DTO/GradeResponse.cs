namespace MemoriesBack.DTO
{
    public class GradeResponse
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Description { get; set; }
        public string TeacherName { get; set; }

        public GradeResponse(int id, int grade, string description, string teacherName)
        {
            Id = id;
            Grade = grade;
            Description = description;
            TeacherName = teacherName;
        }
    }
}