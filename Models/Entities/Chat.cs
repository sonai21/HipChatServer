namespace HipChatServer.Models.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Message?> Messages { get; set; }
}
