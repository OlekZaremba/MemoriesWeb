namespace MemoriesBack.DTO
{
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
}