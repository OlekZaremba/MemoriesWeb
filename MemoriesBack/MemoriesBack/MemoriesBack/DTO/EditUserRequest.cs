using MemoriesBack.Entities;

namespace MemoriesBack.DTO
{
    public class EditUserRequest
    {
        public string Login { get; set; }
        public string Email { get; set; }  // <- DODAJ TO
        public string Name { get; set; }
        public string Surname { get; set; }
        public User.Role Role { get; set; }

        public EditUserRequest() { }

        public EditUserRequest(string login, string email, string name, string surname, User.Role role)
        {
            Login = login;
            Email = email;
            Name = name;
            Surname = surname;
            Role = role;
        }
    }
}