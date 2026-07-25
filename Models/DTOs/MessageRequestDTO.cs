namespace HipChatServer.Models.DTOs;

public class MessageRequestDTO
{
    public Guid? ChatId { get; set; }
    public string? Content { get; set; } 
}
