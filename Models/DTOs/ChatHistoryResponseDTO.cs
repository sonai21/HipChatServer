using HipChatServer.Models.Entities;

namespace HipChatServer.Models.DTOs;

public class ChatHistoryResponseDTO
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ChatMessageDTO> Messages { get; set; }
}
