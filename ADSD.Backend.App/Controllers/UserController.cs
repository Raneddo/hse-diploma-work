using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
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
    public IActionResult GetUsersList()
    {
        return Json(_userService.GetUsers());
    }

    [HttpGet("{id:int}")]
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
    public IActionResult CreateUser([FromBody] UserFullInfo userFullInfo)
    {
        var userId = _userService.CreateUser(userFullInfo);
        return Json(userId);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateUser([FromRoute] int id, [FromBody] UserFullInfo userFullInfo)
    {
        _userService.UpdateUser(id, userFullInfo);
        return Ok();
    }
}