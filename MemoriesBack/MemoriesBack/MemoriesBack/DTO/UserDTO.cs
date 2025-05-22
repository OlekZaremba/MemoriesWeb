using MemoriesBack.Entities;

namespace MemoriesBack.DTO
{
    public record UserDTO(int Id, string Name, string Surname, User.Role Role);
}