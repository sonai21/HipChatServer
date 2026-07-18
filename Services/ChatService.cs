using HipChatServer.Models.DTOs;
using HipChatServer.Models.Entities;
using HipChatServer.RepositoryContracts;
using HipChatServer.ServiceContracts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;

namespace HipChatServer.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly Kernel _kernel;
    private readonly IServiceScopeFactory _scopeFactory;

    public ChatService(IChatRepository chatRepository, Kernel kernel, IServiceScopeFactory scopeFactory)
    {
        _chatRepository = chatRepository;
        _kernel = kernel;
        _scopeFactory = scopeFactory;
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
                RecentMessageCount = 1,
            };
            await _chatRepository.AddChatAsync(chat);
        }
        else
        {
            var existingChat = await _chatRepository.GetChatByIdAsync(id);
            if (existingChat == null) throw new Exception("Chat session not found.");
            chat = existingChat;
            chat.RecentMessageCount = chat.RecentMessageCount + 1;
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
        string systemPrompt = string.IsNullOrWhiteSpace(chat.ChatSummary)
        ? "You are a helpful assistant."
        : $"You are a helpful assistant. Here is a summary of the conversation so far: {chat.ChatSummary}";
        var chatHistory = new ChatHistory(systemPrompt);

        var recentMessages = pastMessages.TakeLast(chat.RecentMessageCount - 1);

        foreach (var msg in recentMessages)
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
        if (chat.RecentMessageCount >= 10)
        {
            var rawHistory = pastMessages.Where(m => m.Role == "user");
            var unSummarizedUserMessages = rawHistory.TakeLast(chat.RecentMessageCount);
            var messageToSummarize = unSummarizedUserMessages.Take(5);
            var summarizeContent = string.Join("\n", messageToSummarize.Select(m => m.Content));
           _= Task.Run(() => GenerateAndSaveChatSummary(chat.Id, summarizeContent));
        }

        MessageResponseDTO aiResponse = new MessageResponseDTO
        {
            ChatId = aiMessage.ChatId,
            AiResponse = aiMessage.Content
        };

        return aiResponse;
    }

    //method for chat summary creation when recent messgaes hits 10.
    private async Task GenerateAndSaveChatSummary(Guid chatId, string pastHistory)
    {
        try
        {
            // Open a brand new background service scope
            using var scope = _scopeFactory.CreateScope();

            // Grab fresh instances of your dependencies
            var bgRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
            //var bgCompletionService = scope.ServiceProvider.GetRequiredService<IChatCompletionService>();
            var bgKernel = scope.ServiceProvider.GetRequiredService<Kernel>();
            var bgCompletionService = bgKernel.GetRequiredService<IChatCompletionService>();
            var bgChat = await bgRepo.GetChatByIdAsync(chatId);
            if (bgChat == null) return;
            //Summary Generation
            var summaryPrompt = new ChatHistory("You are a summarization AI. Your job is to read a log of messages sent by a user and summarize their main topics, goals, and key facts in a short content for ai context.");
            if (!string.IsNullOrWhiteSpace(bgChat.ChatSummary))
            {
                // Give the AI the old summary, plus the new messages, and ask it to merge them
                summaryPrompt.AddUserMessage($"Here is the existing summary of the user's previous messages so far:\n{bgChat.ChatSummary}");
                summaryPrompt.AddUserMessage($"Please update the summary above by incorporating these new messages from the user:\n\n{pastHistory}");
            }
            else
            {
                summaryPrompt.AddUserMessage($"Here are the user's recent messages:\n\n{pastHistory}");
            }

            var summary = await bgCompletionService.GetChatMessageContentAsync(summaryPrompt, kernel: bgKernel);

            //update database
            if (bgChat != null)
            {
                bgChat.ChatSummary = summary.Content;
                bgChat.RecentMessageCount = bgChat.RecentMessageCount - 5;
                await bgRepo.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to summarize chat {chatId}: {ex.Message}");
        }
    }
}
