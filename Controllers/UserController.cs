using DotdotTest.Model.User;
using DotdotTest.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotdotTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        var token = await _userService.LoginUser(request.UserName,request.Password);
        return Ok(token);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var me = _userService.GetMe();
        return Ok(me);
    }

    [HttpPost("superadmin")]
    public async Task<IActionResult> CreateSuperadmin()
    {
        await _userService.CreateSuperadmin();
        return Ok();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        await _userService.CreateUser(request);
        return Ok();
    }

    [HttpPut("")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        await _userService.UpdateUser(request);
        return Ok();
    }

    [HttpDelete("")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromBody] Guid userId)
    {
        await _userService.DeleteUser(userId);
        return Ok();
    }

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetPaginatedUser([FromQuery] GetPagedUserRequest request)
    {
        var result = await _userService.GetPaginatedUser(request);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        var id = Guid.Parse(userId);
        var result = await _userService.GetUser(id);
        return Ok(result);
    }
}
