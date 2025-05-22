namespace MemoriesBack.DTO
{
    public class SchoolClassDTO
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public double Average { get; set; }

        public SchoolClassDTO(int id, string className, double average)
        {
            Id = id;
            ClassName = className;
            Average = average;
        }
    }
}