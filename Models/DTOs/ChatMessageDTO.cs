namespace HipChatServer.Models.DTOs;

public class ChatMessageDTO
{
    public Guid Id { get; set; }
    public string? Role { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
