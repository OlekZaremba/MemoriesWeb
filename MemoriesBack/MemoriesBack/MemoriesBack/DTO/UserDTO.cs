public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Role { get; set; }
    public string? Subject { get; set; }

    public UserDTO(int id, string name, string surname, string role, string? subject = null)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Role = role;
        Subject = subject;
    }
}