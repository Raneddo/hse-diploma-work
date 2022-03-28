using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
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
    [HttpGet("{key}")]
    public IActionResult GetInfo([FromRoute] string key)
    {
        return Json(_infoService.GetInfoByKey(key));
    }

    [HttpPut("")]
    public IActionResult UpdateInfo([FromBody] InfoJson infoJson)
    {
        _infoService.UpdateOrInsertInfo(infoJson.Key, infoJson.Text);
        return Ok();
    }
}