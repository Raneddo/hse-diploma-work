using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
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
    public JsonResult GetAgendasList()
    {
        return Json(_agendaService.GetAgendasList());
    }

    [HttpGet("{id:int}")]
    public JsonResult GetAgendaInfo([FromRoute] int id)
    {
        return Json(_agendaService.GetAgendaInfo(id));
    }

    [HttpPost]
    public JsonResult AddAgenda([FromBody] UpdateAgendaRequest updateAgendaRequest)
    {
        var id = _agendaService.CreateAgenda(updateAgendaRequest);

        return Json(new {Id = id});
    }
    
    [HttpPut("{id:int}")]
    public JsonResult UpdateAgenda([FromBody] UpdateAgendaRequest updateAgendaRequest, [FromRoute] int id)
    {
        _agendaService.UpdateAgenda(id, updateAgendaRequest);

        return Json(new {Id = id});
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteAgenda([FromRoute] int id)
    {
        _agendaService.DeleteAgenda(id);
        return Ok();
    }
}