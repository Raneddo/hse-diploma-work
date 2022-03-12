using ADSD.Backend.App.Json;
using ADSD.Backend.App.Models;
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
    public JsonResult GetAgendasList(int count = int.MaxValue, int offset = 0)
    {
        return Json(_agendaService.GetAgendasList(count, offset));
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

    [HttpGet("{agendaId:int}/poll")]
    public JsonResult GetPolls([FromRoute] int agendaId)
    {
        return Json(new
        {
            Id = 1,
            Text = "Poll text",
            Options = new[]
            {
                new
                {
                    Id = 1,
                    Text = "Option 1",
                    Count = 4
                },
                new
                {
                    Id = 2,
                    Text = "Option 2",
                    Count = 1
                }
            }
        });
    }

    [HttpPost("{agendaId:int}/poll/{pollId:int}")]
    public JsonResult AddPoll([FromRoute] int agendaId, [FromRoute] int pollId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{agendaId:int}/poll/{pollId:int}")]
    public JsonResult UpdatePoll([FromRoute] int agendaId, int pollId)
    {
        throw new NotImplementedException();
    }
}