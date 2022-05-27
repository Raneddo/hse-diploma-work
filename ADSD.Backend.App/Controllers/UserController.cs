using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserBaseInfo>), 200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public IActionResult GetUsersList()
    {
        return Json(_userService.GetUsers());
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(UserFullInfo), 200)]
    [ProducesResponseType(401)]
    [HttpGet("profile")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.GetUserId();
        return userId.HasValue 
            ? GetUser(userId.Value) 
            : Unauthorized();
    }
    
    [HttpGet("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(UserFullInfo), 200)]
    public IActionResult GetUser([FromRoute] int id)
    {
        var user = _userService.GetUser(id);
        if (user != null)
        {
            return Json(user);
        }

        return NotFound();
    }

    [HttpPost("")]
    [Obsolete("Use /register and PUT User/{id}")]
    public IActionResult CreateUser([FromBody] UserFullInfo userFullInfo)
    {
        var userId = _userService.CreateUser(userFullInfo);
        return Json(userId);
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public IActionResult UpdateUser([FromRoute] int id, [FromBody] UserFullInfo userFullInfo)
    {
        _userService.UpdateUser(id, userFullInfo, true);
        return Ok();
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [HttpPut("profile")]
    public IActionResult UpdateUser([FromBody] UserFullInfo userFullInfo)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        _userService.UpdateUser(userId.Value, userFullInfo, false);
        return Ok();
    }
}