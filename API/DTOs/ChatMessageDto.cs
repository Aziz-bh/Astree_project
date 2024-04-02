namespace API.DTOs
{
  public class ChatMessageDto
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; }
}

   public class ChatRoomWithMessagesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ChatMessageDto> Messages { get; set; }
    }
}