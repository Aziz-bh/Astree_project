using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IChatService
    {
        Task AddMessageAsync(int userId, SendMessageDto messageDto);
        Task<int> FindOrCreateChatRoomForUserAsync(string userEmail);
        Task<bool> ChatRoomExists(int chatRoomId);
        Task<ChatRoomWithMessagesDto> GetOrCreateChatRoomForUserAsync(string userEmail);
    }
}
