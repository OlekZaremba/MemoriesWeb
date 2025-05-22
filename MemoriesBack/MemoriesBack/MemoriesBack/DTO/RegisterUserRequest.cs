using MemoriesBack.Entities;

namespace MemoriesBack.DTO
{
    public class RegisterUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public User.Role Role { get; set; }
        public int GroupId { get; set; }

        public RegisterUserRequest() { }
    }
}