namespace API.DTOs
{
  public class ChatMessageDto
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; } // For display purposes
}
}