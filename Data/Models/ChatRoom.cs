namespace Data.Models
{
    public class ChatRoom
{
    public int Id { get; set; }
    public string Name { get; set; } // Optional, for identification
    public ICollection<ChatMessage> Messages { get; set; }
}
}