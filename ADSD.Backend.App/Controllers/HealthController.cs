using ADSD.Backend.App.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("[controller]/")]
public class HealthController : Controller
{
    private readonly SessionTokenDbClient _sessionTokenDbClient;

    public HealthController(SessionTokenDbClient sessionTokenDbClient)
    {
        _sessionTokenDbClient = sessionTokenDbClient;
    }
    
    [HttpGet("check/")]
    public async Task<IActionResult> CheckAuth()
    {
        var authData = await _sessionTokenDbClient.GetUserByToken(Request.Headers.Authorization);

        if (authData != null)
        {
            return Json(new
            {
                Health = "OK",
                UserName = authData.UserName,
            });
        }

        return Unauthorized();
    }
    
    [HttpGet("check/noAuth")]
    public IActionResult CheckNoAuth()
    {
        return Ok();
    }
}

