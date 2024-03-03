using System.Security.Claims;
using API.DTOs;
using API.Models;
using API.Persistence;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

public class ChatController : BaseApiController
{
    private readonly ChatService _chatService;
    private readonly UserManager<User> _userManager;
    private readonly AstreeDbContext _context;

    public ChatController(ChatService chatService, UserManager<User> userManager, AstreeDbContext context)
    {
        _chatService = chatService;
        _userManager = userManager;
        _context = context;
    }

[HttpPost("send")]
[Authorize(Roles = "Member,Admin")]
public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
{
    var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
    // Find or create a chat room based on the user's participation
    var chatRoomId = await _chatService.FindOrCreateChatRoomForUserAsync(email);

    if (chatRoomId <= 0)
    {
        return BadRequest("Unable to find or create a chat room.");
    }

    // Now that we have a valid chatRoomId, we can proceed to send the message
    // Ensure the DTO has the correct chatRoomId
    messageDto.ChatRoomId = chatRoomId;
    
    // Assuming AddMessageAsync accepts a userId and SendMessageDto
    // The user's ID needs to be resolved; assuming UserManager can do this
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
        return Unauthorized("User not found.");
    }

    await _chatService.AddMessageAsync(user.Id, messageDto);

    return Ok();
}



    [HttpGet("{chatRoomId}")]
    [Authorize(Roles = "Member,Admin")]
    public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessages(int chatRoomId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderBy(m => m.Timestamp)
            .Select(m => new ChatMessageDto
            {
                Content = m.Content,
                Timestamp = m.Timestamp,
                UserName = m.User.UserName
            })
            .ToListAsync();

        return Ok(messages);
    }
}

}