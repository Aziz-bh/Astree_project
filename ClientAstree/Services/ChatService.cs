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
            private readonly IHttpContextAccessor _httpContextAccessor;

                 public ChatService(IHttpContextAccessor httpContextAccessor,IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            _httpContextAccessor = httpContextAccessor;
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }

        public async Task SendAsync(SendMessageDto message)
        {
             AddBearerToken();
                     var username = _httpContextAccessor.HttpContext.User.Identity.Name;
                         for (int i = 0; i < 5; i++)
    {
        Console.WriteLine(username);
    }
        var claims = _httpContextAccessor.HttpContext.User.Claims;
    foreach (var claim in claims)
    {
        if(claim.Type=="role")
         if(claim.Value=="Admin")
          await _client.Admin2Async(message.ChatRoomId,username,message);
        else
        await _client.Send2Async(username,message);
    }

            
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