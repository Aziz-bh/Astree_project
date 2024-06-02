using ClientAstree.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using ClientAstree.Contracts;
using ClientAstree.Services.Base;

namespace ClientAstree.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly BadWordFilterService _badWordFilterService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, BadWordFilterService badWordFilterService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _badWordFilterService = badWordFilterService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var chatRoomId = Context.GetHttpContext().Request.Query["chatRoomId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{chatRoomId}");
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(int chatRoomId, string userName, string message)
        {
            _logger.LogInformation($"Received message: {message}");

            // Filter bad words from the message
            var filteredMessage = _badWordFilterService.FilterBadWords(message);

            _logger.LogInformation($"Filtered message: {filteredMessage}");

            await Clients.Group($"room-{chatRoomId}").SendAsync("ReceiveMessage", userName, filteredMessage);
            await _chatService.SendAsync(new SendMessageDto { ChatRoomId = chatRoomId, Content = filteredMessage });
        }
    }
}
