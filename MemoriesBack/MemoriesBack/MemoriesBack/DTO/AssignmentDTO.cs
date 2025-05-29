public class AssignmentDTO
{
    public int AssignmentId { get; set; }
    public string TeacherName { get; set; }
    public string SubjectName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }

    public AssignmentDTO(int assignmentId, string teacherName, string subjectName, int classId, string className)
    {
        AssignmentId = assignmentId;
        TeacherName = teacherName;
        SubjectName = subjectName;
        ClassId = classId;
        ClassName = className;
    }
}