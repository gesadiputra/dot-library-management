namespace DotdotTest.Model.User;

public class UpdateUserRequest
{
    public Guid UserId { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
