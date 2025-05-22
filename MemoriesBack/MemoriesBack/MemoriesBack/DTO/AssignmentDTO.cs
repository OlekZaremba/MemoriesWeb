namespace MemoriesBack.DTO
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }
        public string TeacherName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public AssignmentDTO(int assignmentId, string teacherName, int classId, string className)
        {
            AssignmentId = assignmentId;
            TeacherName = teacherName;
            ClassId = classId;
            ClassName = className;
        }
    }
}