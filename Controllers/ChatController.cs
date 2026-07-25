using HipChatServer.Models.DTOs;
using HipChatServer.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HipChatServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("askQuestion")]
    public async Task<IActionResult> AskQuestion(MessageRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Prompt cannot be empty!");
        }
        try
        {
            var aiGeneratedText = await _chatService.ProcessUserMessageAsync(request.ChatId.Value, request.Content);
            if (aiGeneratedText == null) return NotFound("Chat session not found.");
            return Ok(new { role = "ai", content=aiGeneratedText.AiResponse, chatId = aiGeneratedText.ChatId});
        }
        catch (Exception ex) {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getAllChats")]
    public async Task<IActionResult> GetAllChats()
    {
        var allChats = await _chatService.GetAllChats();
        return Ok(allChats);
    }

    [HttpGet("getChatByChatId")]
    public async Task<IActionResult> GetChatHistory(Guid chatId)
    {
        if (chatId == Guid.Empty)
        {
            return BadRequest("Chat ID is required."); 
        }

        var chatHistory = await _chatService.GetChatHistory(chatId);

        if (chatHistory == null)
        {
            return NotFound("Chat not found.");
        }

        return Ok(chatHistory);
    }
}
