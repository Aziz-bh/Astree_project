using System.Security.Claims;
using API.DTOs;
using Data.Models;
using Data.Persistence;
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

        public ChatController(
            ChatService chatService,
            UserManager<User> userManager,
            AstreeDbContext context
        )
        {
            _chatService = chatService;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("send")]
        [Authorize(Roles = "Member,Admin")]
        // [Authorize(Roles = "Member,Admin")]
        public async Task<IActionResult>
        SendMessage([FromBody] SendMessageDto messageDto)
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chatRoomId =
                await _chatService.FindOrCreateChatRoomForUserAsync(email);

            if (chatRoomId <= 0)
            {
                return BadRequest("Unable to find or create a chat room.");
            }

            messageDto.ChatRoomId = chatRoomId;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            await _chatService.AddMessageAsync(user.Id, messageDto);

            return Ok();
        }


[HttpPost("send/{email}")]
[Authorize(Roles = "Member,Admin")]
public async Task<IActionResult> SendMessage2([FromBody] SendMessageDto messageDto, [FromRoute] string email)
{
    var chatRoomId = await _chatService.FindOrCreateChatRoomForUserAsync(email);

    if (chatRoomId <= 0)
    {
        return BadRequest("Unable to find or create a chat room.");
    }

    messageDto.ChatRoomId = chatRoomId;

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
        // [Authorize(Roles = "Member,Admin")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>>
        GetMessages(int chatRoomId)
        {
            var messages =
                await _context
                    .ChatMessages
                    .Where(m => m.ChatRoomId == chatRoomId)
                    .OrderBy(m => m.Timestamp)
                    .Select(m =>
                        new ChatMessageDto {
                            Content = m.Content,
                            Timestamp = m.Timestamp,
                            UserName = m.User.UserName
                        })
                    .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send/admin/{chatRoomId}")]
        [Authorize(Roles = "Member,Admin")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult>
        SendAdminMessage(int chatRoomId, [FromBody] SendMessageDto messageDto)
        {
            // This ensures that only admins can access this method
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Only admins can send messages to any chat room.");
            }

            // Check if the chat room exists
            var chatRoomExists = await _chatService.ChatRoomExists(chatRoomId);
            if (!chatRoomExists)
            {
                return NotFound($"ChatRoom with Id {chatRoomId} not found.");
            }

            var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Setting the chatRoomId from the method parameter
            messageDto.ChatRoomId = chatRoomId;

            await _chatService.AddMessageAsync(user.Id, messageDto);

            return Ok();
        }



[HttpPost("admin/{chatRoomId}")]
// [Authorize(Roles = "Member,Admin")]
public async Task<IActionResult> SendAdminMessage2(int chatRoomId, [FromBody] SendMessageDto messageDto, [FromQuery] string userEmail)
{
    // Ensure only admins can access this method
    // if (!User.IsInRole("Admin"))
    // {
    //     return Unauthorized("Only admins can send messages to any chat room.");
    // }

    // Check if the chat room exists
    var chatRoomExists = await _chatService.ChatRoomExists(chatRoomId);
    if (!chatRoomExists)
    {
        return NotFound($"ChatRoom with Id {chatRoomId} not found.");
    }

    var user = await _userManager.FindByEmailAsync(userEmail);
    if (user == null)
    {
        return Unauthorized("User not found.");
    }

    // Setting the chatRoomId from the method parameter
    messageDto.ChatRoomId = chatRoomId;

    await _chatService.AddMessageAsync(user.Id, messageDto);

    return Ok();
}


        [HttpGet("chatrooms")]
       // [Authorize(Roles = "Member,Admin")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ChatRoomDto>>>
        GetAllChatRooms()
        {
            // This ensures that only admins can access this method
            // if (!User.IsInRole("Admin"))
            // {
            //     return Unauthorized("Only admins can access chat room details.");
            // }

            var chatRooms =
                await _context
                    .ChatRooms
                    .Select(cr =>
                        new ChatRoomDto { Id = cr.Id, Name = cr.Name })
                    .ToListAsync();

            return Ok(chatRooms);
        }

        [HttpGet("userchatroom")]
        
        // [Authorize(Roles = "Member")]
        public async Task<ActionResult<ChatRoomWithMessagesDto>> GetMyChatRoom()
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chatRoomDto =
                await _chatService.GetOrCreateChatRoomForUserAsync(email);
            return Ok(chatRoomDto);
        }
    }
}
