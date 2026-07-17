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
            var aiGeneratedText = await _chatService.ProcessUserMessageAsync(request.ChatId, request.Content);
            if (aiGeneratedText == null) return NotFound("Chat session not found.");
            return Ok(new { role = "ai", content=aiGeneratedText.AiResponse, chatId = aiGeneratedText.ChatId});
        }
        catch (Exception ex) {
            return StatusCode(500, ex.Message);
        }
    }
}
