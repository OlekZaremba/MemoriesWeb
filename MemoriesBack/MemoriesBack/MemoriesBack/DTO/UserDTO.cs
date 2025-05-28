public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Role { get; set; }
    public string? Email { get; set; } // Dodaj email
    public string? Subject { get; set; }

    public UserDTO(int id, string name, string surname, string role, string? email = null, string? subject = null)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Role = role;
        Email = email;
        Subject = subject;
    }
}