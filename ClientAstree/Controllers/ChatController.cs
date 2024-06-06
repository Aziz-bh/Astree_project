using ClientAstree.Contracts;
using ClientAstree.Services.Base;
using ClientAstree.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;

namespace ClientAstree.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly BadWordFilterService _badWordFilterService;

        public ChatController(IChatService chatService, BadWordFilterService badWordFilterService)
        {
            _chatService = chatService;
            _badWordFilterService = badWordFilterService;
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

        // New method to display the addWords view
        public IActionResult AddWords()
        {
            return View();
        }

        // New method to handle adding a word to the Excel file
        [HttpPost]
        public async Task<IActionResult> AddWord(string word)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Words.xlsx");

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Words");

                // Find the first empty row
                var row = 1;
                while (worksheet.Cells[row, 1].Value != null)
                {
                    row++;
                }

                worksheet.Cells[row, 1].Value = word;

                // Save the Excel package
                package.Save();
            }

            return RedirectToAction("AddWords");
        }
    }
}
