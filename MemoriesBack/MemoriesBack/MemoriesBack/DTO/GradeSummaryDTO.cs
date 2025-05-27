namespace MemoriesBack.DTO
{
    public class GradeSummaryDTO
    {
        public int Id { get; set; }
        public double Grade { get; set; }
        public string Type { get; set; }
        public string IssueDate { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }

        public GradeSummaryDTO(int id, double grade, string type, string issueDate, string className, string description)
        {
            Id = id;
            Grade = grade;
            Type = type;
            IssueDate = issueDate;
            ClassName = className;
            Description = description;
        }

    }
}