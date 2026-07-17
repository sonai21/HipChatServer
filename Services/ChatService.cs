using HipChatServer.Models.DTOs;
using HipChatServer.Models.Entities;
using HipChatServer.RepositoryContracts;
using HipChatServer.ServiceContracts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HipChatServer.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly Kernel _kernel;

    public ChatService(IChatRepository chatRepository,Kernel kernel)
    {
        _chatRepository = chatRepository;
        _kernel = kernel;
    }
    public async Task<MessageResponseDTO?> ProcessUserMessageAsync(Guid id, string content)
    {
        Chat chat;
        if (id == null || id == Guid.Empty)
        {
            chat = new Chat()
            {
                Id = Guid.NewGuid(),
                Title = content.Length > 30 ? content.Substring(0, 30) + "..." : content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            await _chatRepository.AddChatAsync(chat);
        }
        else
        {
            var existingChat = await _chatRepository.GetChatByIdAsync(id);
            if (existingChat == null) throw new Exception("Chat session not found.");
            chat = existingChat;
        }

        var userMessage = new Message
        {
            ChatId = chat.Id,
            Role = "user",
            Content = content,
            CreatedAt = DateTime.UtcNow,
        };

        await _chatRepository.AddMessgaeAsync(userMessage);
        var pastMessages = await _chatRepository.GetChatHistoryAsync(chat.Id);
        var chatHistory = new ChatHistory("You are a helpful assistant.");

        foreach (var msg in pastMessages)
        {
            if (msg.Role == "user") chatHistory.AddUserMessage(msg.Content);
            else chatHistory.AddAssistantMessage(msg.Content);
        }
        chatHistory.AddUserMessage(content);
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, kernel: _kernel);
        var aiMessage = new Message
        {
            ChatId = chat.Id,
            Role = "ai",
            Content = response.Content ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        chat.UpdatedAt = DateTime.UtcNow;
        await _chatRepository.SaveChangesAsync();

        MessageResponseDTO aiResponse = new MessageResponseDTO
        {
            ChatId = aiMessage.ChatId,
            AiResponse = aiMessage.Content
        };

        return aiResponse;
    }
}
