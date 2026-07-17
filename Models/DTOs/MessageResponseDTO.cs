namespace HipChatServer.Models.DTOs;

public class MessageResponseDTO
{
    public Guid ChatId { get; set; }
    public string AiResponse { get; set; }
}
