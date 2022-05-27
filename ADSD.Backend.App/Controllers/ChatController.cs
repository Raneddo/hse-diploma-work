using System.ComponentModel.DataAnnotations;
using System.Net;
using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Models;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.App.Controllers;

[Route("/api/[controller]")]
public class ChatController : Controller
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<ChatShortInfo>), 200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public IActionResult GetUserChats()
    {
        var userId = User.GetUserId() ?? 0;
        if (userId == 0)
        {
            return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
        }

        return Json(_chatService.GetChatsByUser(userId));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(long), 200)]
    [HttpPost("personal")]
    public IActionResult CreatePersonalChat(int toUserId)
    {
        var userId = User.GetUserId() ?? 0;
        if (userId == 0)
        {
            return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
        }

        return Json(_chatService.CreatePersonalChat(userId, toUserId));
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost("group")]
    public IActionResult CreateGroupChat([FromBody] ChatGroupCreateRequest request)
    {
        var userId = User.GetUserId() ?? 0;
        if (userId == 0)
        {
            return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
        }

        return Json(_chatService.CreateGroupChat(request.Name, userId, request.UserIds.DefaultIfEmpty().ToArray()));
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost("channel")]
    public IActionResult CreateChannel([FromBody] ChatChannelCreateRequest request)
    {
        return Json(_chatService.CreateChannel(request.Name));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [HttpPost("{chatId:long}/send")]
    public IActionResult SendMessage([FromRoute] long chatId, [FromBody] ChatMessageSendRequest request)
    {
        var userId = User.GetUserId() ?? 0;
        if (userId == 0)
        {
            return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
        }

        var isAdmin = User.GetUserRoles().Contains(UserRole.Admin);

        try
        {
            _chatService.SendMessage(isAdmin, userId, chatId, request.Message);
            return Ok();
        }
        catch (ArgumentOutOfRangeException)
        {
            return new StatusCodeResult((int) HttpStatusCode.Forbidden);
        }
        catch (ArgumentNullException)
        {
            return new StatusCodeResult((int) HttpStatusCode.NotFound);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(IEnumerable<ChatMessage>), 200)]
    [HttpGet("{chatId:long}/messages")]
    public IActionResult GetChatMessages([FromRoute] long chatId, long afterMessageId = 0, long count = int.MaxValue)
    {
        var userId = User.GetUserId() ?? 0;
        if (userId == 0)
        {
            return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
        }

        try
        {
            return Json(_chatService.GetChatMessages(chatId, userId, afterMessageId, count));
        }
        catch (HttpCodeException e)
        {
            var response = Json(e.Message);
            response.StatusCode = e.StatusCode;
            return response;
        }
    }

    // [HttpGet("")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    // public IActionResult GetMessages(int chatId, int offsetId)
    // {
    //     var userId = User.GetUserId() ?? 0;
    //     if (userId == 0)
    //     {
    //         return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
    //     }
    //     
    //     _chatService.GetMessages()
    // }
}