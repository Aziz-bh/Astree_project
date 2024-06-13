using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using Data.Models;
using Data.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ChatController : BaseApiController
    {
        private readonly IChatService _chatService;
        private readonly UserManager<User> _userManager;
        private readonly AstreeDbContext _context;

        public ChatController(
            IChatService chatService,
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
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

        [HttpPost("send/admin/{chatRoomId}")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<IActionResult> SendAdminMessage(int chatRoomId, [FromBody] SendMessageDto messageDto)
        {
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Only admins can send messages to any chat room.");
            }

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

            messageDto.ChatRoomId = chatRoomId;

            await _chatService.AddMessageAsync(user.Id, messageDto);

            return Ok();
        }

        [HttpPost("admin/{chatRoomId}")]
        public async Task<IActionResult> SendAdminMessage2(int chatRoomId, [FromBody] SendMessageDto messageDto, [FromQuery] string userEmail)
        {
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

            messageDto.ChatRoomId = chatRoomId;

            await _chatService.AddMessageAsync(user.Id, messageDto);

            return Ok();
        }

        [HttpGet("chatrooms")]
        public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetAllChatRooms()
        {
            var chatRooms = await _context.ChatRooms
                .Select(cr => new ChatRoomDto { Id = cr.Id, Name = cr.Name })
                .ToListAsync();

            return Ok(chatRooms);
        }

        [HttpGet("userchatroom")]
        public async Task<ActionResult<ChatRoomWithMessagesDto>> GetMyChatRoom()
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chatRoomDto = await _chatService.GetOrCreateChatRoomForUserAsync(email);
            return Ok(chatRoomDto);
        }
    }
}
