namespace Data.Models
{
   public class ChatMessage
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } 
    public int ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }
}
}