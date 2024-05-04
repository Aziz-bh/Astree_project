using Microsoft.AspNetCore.SignalR;
using ClientAstree.Contracts;
using ClientAstree.Services.Base; // Ensure the service contract namespace is included
namespace ClientAstree.Hubs{
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        // Example of adding to a group based on a query string parameter
        var chatRoomId = Context.GetHttpContext().Request.Query["chatRoomId"];
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{chatRoomId}");
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(int chatRoomId, string userName, string message)
    {
        await Clients.Group($"room-{chatRoomId}").SendAsync("ReceiveMessage", userName, message);
        await _chatService.SendAsync(new SendMessageDto { ChatRoomId = chatRoomId, Content = message });
    }
}

}