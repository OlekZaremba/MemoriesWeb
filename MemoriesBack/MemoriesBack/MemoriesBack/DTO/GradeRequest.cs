namespace MemoriesBack.DTO
{
    public class GradeRequest
    {
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }

        public double Grade { get; set; } 

        public string? Description { get; set; }
        public string? Type { get; set; }

        public GradeRequest() { }
    }
}