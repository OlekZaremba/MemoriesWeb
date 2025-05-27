public class GroupStudentWithGradesDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<GradeSimpleDTO> Grades { get; set; }
    public double Average { get; set; }

    public GroupStudentWithGradesDTO(int id, string name, string surname, List<GradeSimpleDTO> grades, double average)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Grades = grades;
        Average = average;
    }
}

public class GradeSimpleDTO
{
    public int Id { get; set; }
    public double Value { get; set; }
    public string Type { get; set; }
    public string Comment { get; set; }
    public string IssueDate { get; set; }

    public GradeSimpleDTO(int id, double value, string type, string comment, string issueDate)
    {
        Id = id;
        Value = value;
        Type = type;
        Comment = comment;
        IssueDate = issueDate;
    }
}
