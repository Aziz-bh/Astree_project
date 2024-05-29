using ClientAstree.Contracts;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Index()
        {
            var chatRooms = await _chatService.GetAllChatRoomsAsync();
            return View(chatRooms);
        }

public async Task<IActionResult> ChatRoom(int id)
{
    var roomDetails = await _chatService.GetMessagesAsync(id);
    if (roomDetails == null)
    {
        return NotFound();
    }
    ViewBag.ChatRoomId = id;  // Ensure this is set correctly
    return View(roomDetails);
}



[HttpPost]
public async Task<IActionResult> SendMessage(SendMessageDto message)
{
    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
    {
        await _chatService.SendAsync(message);
        return Json(new { success = true, message = "Message sent successfully" });
    }

    await _chatService.SendAsync(message);
    // Redirecting back to the AdminChatRoom to refresh the messages
    return RedirectToAction("AdminChatRoom");
}


        [HttpPost]
        public async Task<IActionResult> AdminSendMessage(int chatRoomId, SendMessageDto message)
        {
            await _chatService.AdminSendAsync(chatRoomId, message);
            return RedirectToAction("ChatRoom", new { id = chatRoomId });
        }

        public async Task<IActionResult> AdminChatRoom()
{
    // Assuming the GetUserChatRoomWithMessagesAsync method is correctly fetching the admin-user chat room
    var roomDetails = await _chatService.GetChatRoomWithMessagesAsync();
    if (roomDetails == null)
    {
        return NotFound("No chat room available.");
    }
    return View("AdminChatRoom", roomDetails);
}


[HttpPost]
public async Task<IActionResult> ReplyAsAdmin(SendMessageDto message)
{

        await _chatService.AdminSendAsync(message.ChatRoomId, message);
    return RedirectToAction("ChatRoom", new { id = message.ChatRoomId });
}


    }
       }