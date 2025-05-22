namespace MemoriesBack.DTO
{
    public class GroupDTO
    {
        public int Id { get; set; }
        public string GroupName { get; set; }

        public GroupDTO(int id, string groupName)
        {
            Id = id;
            GroupName = groupName;
        }
    }
}