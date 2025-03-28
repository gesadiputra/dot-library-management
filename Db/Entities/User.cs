namespace DotdotTest.Db.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
