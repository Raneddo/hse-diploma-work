using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class PollController : Controller
{
    private readonly PollService _pollService;

    public PollController(PollService pollService)
    {
        _pollService = pollService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PollResponse>), 200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public IActionResult GetPolls()
    {
        var userId = User.GetUserId() ?? 0;
        return Json(_pollService.GetPolls(userId: userId));
    }

    [HttpGet("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public IActionResult GetPoll([FromRoute] int id)
    {
        var userId = User.GetUserId() ?? 0;
        try
        {
            var poll = _pollService.GetPollById(id, userId);
            return Json(poll);
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), 200)]
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [HttpPost("{id:int}/vote")]
    public IActionResult Vote([FromRoute] int id, [FromBody] PollVoteRequest request)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        _pollService.VotePollOptions(id, userId.Value, request.OptionIds);
        return Ok();
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [HttpDelete("{id:int}/vote")]
    public IActionResult Unvote([FromRoute] int id)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        _pollService.UnvotePollOptions(id, userId.Value);
        return Ok();
    }
}