using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class ChatService : BaseHttpService, IChatService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IClient _httpclient;
                 public ChatService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }

        public async Task SendAsync(SendMessageDto message)
        {
             AddBearerToken();
            await _client.SendAsync(message);
        }

        public async Task<ICollection<ChatMessageDto>> GetMessagesAsync(int chatRoomId)
        { AddBearerToken();
            return await _client.ChatAsync(chatRoomId);
        }

        public async Task AdminSendAsync(int chatRoomId, SendMessageDto message)
        { AddBearerToken();
            await _client.AdminAsync(chatRoomId, message);
        }

        public async Task<ICollection<ChatRoomDto>> GetAllChatRoomsAsync()
        { AddBearerToken();
            return await _client.ChatroomsAsync();
        }

        public async Task<ChatRoomWithMessagesDto> GetChatRoomWithMessagesAsync()
        { AddBearerToken();
           return await _client.UserchatroomAsync();
        }
    }
     }