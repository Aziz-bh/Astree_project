using API.DTOs;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
public class ChatService
{
     private readonly AstreeDbContext _context;
         private readonly UserManager<User> _userManager;

    public ChatService(AstreeDbContext context,UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task AddMessageAsync(int userId, SendMessageDto messageDto)
    {
        var message = new ChatMessage
        {
            UserId = userId,
            ChatRoomId = messageDto.ChatRoomId,
            Content = messageDto.Content,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }

public async Task<int> FindOrCreateChatRoomForUserAsync(string userEmail)
{
    var user = await _userManager.FindByEmailAsync(userEmail);
    if (user == null)
    {
        throw new ArgumentException("User not found.");
    }

    // Find any existing chat room that includes messages from this user
    var chatRoomId = await _context.ChatMessages
        .Where(cm => cm.UserId == user.Id)
        .Select(cm => cm.ChatRoomId)
        .FirstOrDefaultAsync();

    if (chatRoomId == 0) // No existing chat room found
    {
        // Create a new chat room
        var chatRoom = new ChatRoom { Name = $"Chat with {user.UserName}" }; // Customize naming as needed
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();

        // Note: At this point, you may want to add a welcome or initial message to the chat room from an admin.

        chatRoomId = chatRoom.Id;
    }

    return chatRoomId;
}


private async Task AddUserToChatRoom(int userId, int chatRoomId, string welcomeMessage)
{
    var chatMessage = new ChatMessage 
    { 
        UserId = userId, 
        ChatRoomId = chatRoomId, 
        Content = welcomeMessage, // This could be a welcome message or similar
        Timestamp = DateTime.UtcNow 
    };
    
    _context.ChatMessages.Add(chatMessage);
    await _context.SaveChangesAsync();
}


    
}

}