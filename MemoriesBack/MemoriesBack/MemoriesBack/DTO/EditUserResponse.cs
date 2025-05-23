using MemoriesBack.Entities;

namespace MemoriesBack.DTO
{
    public class EditUserResponse
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public User.Role Role { get; set; }
        public string Image { get; set; }

        public EditUserResponse(int id, string login, string name, string surname, User.Role role, string image)
        {
            Id = id;
            Login = login;
            Name = name;
            Surname = surname;
            Role = role;
            Image = image;
        }
    }
}