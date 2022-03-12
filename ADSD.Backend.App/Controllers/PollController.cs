using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class PollController : Controller
{
    private readonly AgendaService _agendaService;
    private readonly PollService _pollService;

    public PollController(AgendaService agendaService, PollService pollService)
    {
        _agendaService = agendaService;
        _pollService = pollService;
    }
    
    [HttpGet]
    public IActionResult GetPolls()
    {
        return Json(_pollService.GetPolls(userId: 1));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPoll([FromRoute] int id)
    {
        var poll = _pollService.GetPollById(id, userId: 1);
        if (poll != null)
        {
            return Json(poll);
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult CreatePoll([FromBody] CreatePollRequest req)
    {
        return Json(_pollService.CreatePoll(req.Name, req.Text, req.Options, req.MultiChoice));
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePoll([FromRoute] int id)
    {
        _pollService.DeletePoll(id);
        return Ok();
    }

    [HttpPost("{id:int}/vote")]
    public IActionResult Vote([FromRoute] int id, [FromBody] List<int> options)
    {
        _pollService.VotePollOptions(id, 1, options);
        return Ok();
    }
    
    [HttpDelete("{id:int}/vote")]
    public IActionResult Unvote([FromRoute] int id)
    {
        _pollService.UnvotePollOptions(id, 1);
        return Ok();
    }
}