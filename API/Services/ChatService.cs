using API.DTOs;
using API.Interfaces;
using Data.Models;
using Data.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ChatService : IChatService
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
            if (string.IsNullOrWhiteSpace(messageDto.Content))
            {
                throw new ArgumentException("Message content cannot be empty.");
            }

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

            var chatRoom = await _context.ChatRooms
                .FirstOrDefaultAsync(cr => cr.Name == $"Chat with {userEmail}");

            if (chatRoom != null)
            {
                return chatRoom.Id;
            }

            var chatRoomId = await _context.ChatMessages
                .Where(cm => cm.UserId == user.Id)
                .Select(cm => cm.ChatRoomId)
                .FirstOrDefaultAsync();

            if (chatRoomId == 0)
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

            var chatRoom = await _context.ChatRooms
                .Include(cr => cr.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(cr => cr.Messages.Any(m => m.UserId == user.Id));

            if (chatRoom == null)
            {
                chatRoom = new ChatRoom
                {
                    Name = $"Chat with {user.UserName}",
                    Messages = new List<ChatMessage>()
                };
                _context.ChatRooms.Add(chatRoom);
                await _context.SaveChangesAsync();
            }

            var chatRoomDto = new ChatRoomWithMessagesDto
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name,
                Messages = chatRoom.Messages?.Select(m => new ChatMessageDto
                {
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    UserName = m.User.UserName
                }).ToList() ?? new List<ChatMessageDto>()
            };

            return chatRoomDto;
        }
    }
}
