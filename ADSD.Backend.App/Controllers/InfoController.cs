using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class InfoController : Controller
{
    private readonly InfoService _infoService;

    public InfoController(InfoService infoService)
    {
        _infoService = infoService;
    }
    
    [AllowAnonymous]
    [ProducesResponseType(typeof(InfoResponse), 200)]
    [HttpGet("{key}")]
    public IActionResult GetInfo([FromRoute] string key)
    {
        return Json(_infoService.GetInfoByKey(key));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPut("{key}")]
    public IActionResult UpdateInfo([FromRoute] string key, [FromBody] InfoJsonRequest infoJsonRequest)
    {
        _infoService.UpdateOrInsertInfo(key, infoJsonRequest.Text);
        return Ok();
    }
}