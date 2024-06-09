using API.DTOs;
using Data.Models;
using Data.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class ChatService
    {
        private readonly AstreeDbContext _context;
        private readonly UserManager<User> _userManager;

        public ChatService(AstreeDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddMessageAsync(int userId, SendMessageDto messageDto)
        {
            // Validate the message content
            if (string.IsNullOrWhiteSpace(messageDto.Content))
            {
                throw new ArgumentException("Message content cannot be empty.");
            }

            // Ensure the ChatRoom exists
            var chatRoomExists = await ChatRoomExists(messageDto.ChatRoomId);
            if (!chatRoomExists)
            {
                throw new ArgumentException("ChatRoom does not exist.");
            }

            var message = new ChatMessage
            {
                UserId = userId,
                ChatRoomId = messageDto.ChatRoomId,
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new InvalidOperationException("An error occurred while saving the message.", ex);
            }
        }

public async Task<int> FindOrCreateChatRoomForUserAsync(string userEmail)
{
    var user = await _userManager.FindByEmailAsync(userEmail);
    if (user == null)
    {
        throw new ArgumentException("User not found.");
    }

    // Check if a chat room exists with the user's email in the name
    var chatRoom = await _context.ChatRooms
        .FirstOrDefaultAsync(cr => cr.Name == $"Chat with {userEmail}");

    if (chatRoom != null)
    {
        return chatRoom.Id;
    }

    // Check if a chat room exists where the user has already sent messages
    var chatRoomId = await _context.ChatMessages
        .Where(cm => cm.UserId == user.Id)
        .Select(cm => cm.ChatRoomId)
        .FirstOrDefaultAsync();

    if (chatRoomId == 0) // No existing chat room found
    {
        chatRoom = new ChatRoom { Name = $"Chat with {user.UserName}" };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();
        chatRoomId = chatRoom.Id;
    }

    return chatRoomId;
}


        public async Task<bool> ChatRoomExists(int chatRoomId)
        {
            return await _context.ChatRooms.AnyAsync(cr => cr.Id == chatRoomId);
        }

public async Task<ChatRoomWithMessagesDto> GetOrCreateChatRoomForUserAsync(string userEmail)
{
    var user = await _userManager.FindByEmailAsync(userEmail);
    if (user == null)
    {
        throw new ArgumentException("User not found.");
    }

    // Attempt to find an existing chat room for the user
    var chatRoom = await _context.ChatRooms
        .Include(cr => cr.Messages)
            .ThenInclude(m => m.User)
        .FirstOrDefaultAsync(cr => cr.Messages.Any(m => m.UserId == user.Id));

    if (chatRoom == null)
    {
        // Create a new chat room if not found
        chatRoom = new ChatRoom
        {
            Name = $"Chat with {user.UserName}",
            Messages = new List<ChatMessage>() // Initialize the Messages collection
        };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();
    }

    // Convert to DTO, ensuring to handle null Messages collection appropriately
    var chatRoomDto = new ChatRoomWithMessagesDto
    {
        Id = chatRoom.Id,
        Name = chatRoom.Name,
        Messages = chatRoom.Messages?.Select(m => new ChatMessageDto
        {
            Content = m.Content,
            Timestamp = m.Timestamp,
            UserName = m.User.UserName
        }).ToList() ?? new List<ChatMessageDto>() // Use an empty list if Messages is null
    };

    return chatRoomDto;
}


        
    }
}
