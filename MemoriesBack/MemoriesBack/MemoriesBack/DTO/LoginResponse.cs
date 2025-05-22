using MemoriesBack.Entities;

namespace MemoriesBack.DTO
{
    public record LoginResponse(
        int Id,
        string Name,
        string Surname,
        User.Role Role,
        string Image,
        string ClassName
    );
}