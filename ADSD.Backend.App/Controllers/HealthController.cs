using ADSD.Backend.App.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class HealthController : Controller
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Speaker")]
    [HttpGet("check/")]
    public IActionResult CheckAuth()
    {
        var currentUser = HttpContext.User;

        if (currentUser.IsInRole(UserRole.Admin.ToString()))
        {
            return Json(new {Message = "You are great admin"});
        }

        if (currentUser.IsInRole(UserRole.User.ToString()))
        {
            return Json(new {Message = "You are only user"});
        }

        return Unauthorized();
    }
    
    [HttpGet("check/noAuth")]
    public IActionResult CheckNoAuth()
    {
        return Ok();
    }
}

