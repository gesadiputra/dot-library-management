using DotdotTest.Db.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DotdotTest.Service;

public interface IAuthService
{
    string GetToken(User user);
    User GetUserFromToken();
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContext;
    public AuthService(IConfiguration config, IHttpContextAccessor httpContext)
    {
        _config = config;
        _httpContext = httpContext;
    }
    public string GetToken(User user)
    {
        var secretKey = _config["Jwt:Secret"];
        if (secretKey == null) throw new Exception();

        var signingCredential = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = signingCredential,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }

    public User GetUserFromToken()
    {
        return new User
        {
            Id = Guid.Parse(_httpContext.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
        };
    }
}
