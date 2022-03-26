using System.Security.Claims;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class HealthController : Controller
{
    private readonly SessionTokenDbClient _sessionTokenDbClient;

    public HealthController(SessionTokenDbClient sessionTokenDbClient)
    {
        _sessionTokenDbClient = sessionTokenDbClient;
    }
    
    [HttpGet("check/")]
    [Authorize]
    public IActionResult CheckAuth()
    {
        var currentUser = HttpContext.User;

        if (currentUser.HasClaim(x => x.Type == ClaimsIdentity.DefaultRoleClaimType))
        {
            var roles = currentUser.Claims.First(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (roles.Contains(UserRole.Admin.ToString()))
            {
                return Json(new
                {
                    Message = "Success admin",
                    UserId = currentUser.Claims.FirstOrDefault(c => c.Type == "id")?.Value,
                    Roles = roles.Split(',')
                });
            }
            else if (roles.Contains(UserRole.User.ToString()))
            {
                return Json(new
                {
                    Message = "Success user",
                    UserId = currentUser.Claims.FirstOrDefault(c => c.Type == "id")?.Value,
                    Roles = roles.Split(',')
                });
            }
        }
        return Unauthorized();
    }
    
    [HttpGet("check/noAuth")]
    public IActionResult CheckNoAuth()
    {
        return Ok();
    }
}

