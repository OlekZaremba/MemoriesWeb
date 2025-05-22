namespace MemoriesBack.DTO
{
    public class ClassDTO
    {
        public int Id { get; set; }
        public string ClassName { get; set; }

        public ClassDTO(int id, string className)
        {
            Id = id;
            ClassName = className;
        }
    }
}