using DotdotTest.Db;
using DotdotTest.Db.Entities;
using DotdotTest.Model;
using DotdotTest.Model.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DotdotTest.Service;

public interface IUserService
{
    Task<string> LoginUser(string username, string password);
    User GetMe();
    Task CreateSuperadmin();
    Task CreateUser(CreateUserRequest request);
    Task UpdateUser(UpdateUserRequest request);
    Task DeleteUser(Guid userId);
    Task<BasePagedResponse<User>> GetPaginatedUser(GetPagedUserRequest request);
    Task<User> GetUser(Guid userId);
}
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAuthService _authService;
    public UserService(ApplicationDbContext dbContext, IAuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }
    public async Task<User> GetUser(Guid userId)
    {
        var user = await _dbContext
            .Set<User>()
            .Where(p => p.Id == userId)
            .FirstOrDefaultAsync();
        if (user == null) throw new Exception("User not found");

        return user;
    }
    public async Task<string> LoginUser(string username, string password)
    {
        var user = await _dbContext
            .Set<User>()
            .Where(p => p.Username == username)
            .FirstOrDefaultAsync();
        if (user.Password != password) throw new Exception("Password Incorrect");

        return _authService.GetToken(user);
    }
    public User GetMe()
    {
        return _authService.GetUserFromToken();
    }
    public async Task CreateSuperadmin()
    {
        var existing = await _dbContext
            .Set<User>()
            .Where(p => p.Username == "superadmin")
            .FirstOrDefaultAsync();
        if (existing != null) return;

        var superadmin = new User
        {
            Fullname = "Super Admin",
            Email = "superadmin@mail.com",
            Username = "superadmin",
            Password = "123456"
        };
        await _dbContext.AddAsync(superadmin);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Fullname = request.Fullname,
            Email = request.Email,
            Username = request.Username,
            Password = request.Password
        };
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUser(UpdateUserRequest request)
    {
        var user = await GetUser(request.UserId);
        _dbContext.Attach(user);
        user.Email = request.Email;
        user.Fullname = request.Fullname;
        user.Password = request.Password;
        user.UpdatedBy = _authService.GetUserFromToken().Id;
        user.UpdatedDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await GetUser(userId);
        _dbContext.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<BasePagedResponse<User>> GetPaginatedUser(GetPagedUserRequest request)
    {
        var query = _dbContext
            .Set<User>()
            .AsQueryable();

        if(!string.IsNullOrEmpty(request.Search))
        {
            query = query
                .Where(p => p.Username.Contains(request.Search) || p.Email.Contains(request.Search));
        }

        var totalData = await query.CountAsync();
        var result = await query
            .Skip(request.SkipCount())
            .Take(request.Size)
            .ToListAsync();

        return new BasePagedResponse<User>
        {
            TotalData = totalData,
            ItemCount = result.Count,
            Items = result
        };
    }
}
