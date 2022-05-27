using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class AgendaController : Controller
{
    private readonly AgendaService _agendaService;

    public AgendaController(AgendaService agendaService)
    {
        _agendaService = agendaService;
    }
    
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(IEnumerable<AgendaResponse>), 200)]
    [ProducesResponseType(401)]
    public JsonResult GetAgendasList()
    {
        return Json(_agendaService.GetAgendasList());
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(AgendaResponse), 200)]
    [ProducesResponseType(401)]
    [HttpGet("{id:int}")]
    public JsonResult GetAgendaInfo([FromRoute] int id)
    {
        try
        {
            return Json(_agendaService.GetAgendaInfo(id));
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(401)]
    public JsonResult AddAgenda([FromBody] UpdateAgendaRequest updateAgendaRequest)
    {
        try
        {
            var id = _agendaService.CreateAgenda(updateAgendaRequest);
            return Json(id);
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
        
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AgendaResponse>), 200)]
    [ProducesResponseType(401)]
    [HttpPut("{id:int}")]
    public JsonResult UpdateAgenda([FromBody] UpdateAgendaRequest updateAgendaRequest, [FromRoute] int id)
    {
        try
        {
            _agendaService.UpdateAgenda(id, updateAgendaRequest);
            return Json(new {Id = id});
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
        
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AgendaResponse>), 200)]
    [ProducesResponseType(401)]
    public IActionResult DeleteAgenda([FromRoute] int id)
    {
        try
        {
            _agendaService.DeleteAgenda(id);
            return Ok();
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
    }
}