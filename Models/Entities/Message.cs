namespace HipChatServer.Models.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }

    public string? Role { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Chat? Chat { get; set; }
}
