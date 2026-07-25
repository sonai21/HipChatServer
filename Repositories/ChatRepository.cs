using HipChatServer.Data;
using HipChatServer.Models.Entities;
using HipChatServer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace HipChatServer.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ChatRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddChatAsync(Chat chat)
    {
        await _dbContext.Chats.AddAsync(chat);
    }

    public async Task AddMessgaeAsync(Message message)
    {
        await _dbContext.Messages.AddAsync(message);
    }

    public async Task<List<Chat>> GetAllChatsAsync()
    {
        return await _dbContext.Chats.ToListAsync();
    }

    public async Task<Chat?> GetChatByIdAsync(Guid id)
    {
        return await _dbContext.Chats.FindAsync(id);
    }

    public async Task<List<Message>> GetChatHistoryAsync(Guid id)
    {
        return await _dbContext.Messages.Where(i => i.ChatId == id).OrderBy(i => i.CreatedAt).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
