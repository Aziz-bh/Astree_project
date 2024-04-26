using ClientAstree.Services.Base;

namespace ClientAstree.Contracts
{
    public interface IChatService
    {
        Task SendAsync(SendMessageDto message);
        Task<ICollection<ChatMessageDto>> GetMessagesAsync(int chatRoomId);
        Task AdminSendAsync(int chatRoomId, SendMessageDto message);
        Task<ICollection<ChatRoomDto>> GetAllChatRoomsAsync();
        Task<ChatRoomWithMessagesDto> GetChatRoomWithMessagesAsync();
    }
}