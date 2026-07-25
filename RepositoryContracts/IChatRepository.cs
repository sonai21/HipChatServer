using HipChatServer.Models.Entities;

namespace HipChatServer.RepositoryContracts;

public interface IChatRepository
{
    Task<Chat?> GetChatByIdAsync(Guid id);
    Task<List<Chat>> GetAllChatsAsync();
    Task AddMessgaeAsync (Message message);

    Task<List<Message>> GetChatHistoryAsync(Guid id);

    Task AddChatAsync(Chat chat);

    Task SaveChangesAsync();
}
