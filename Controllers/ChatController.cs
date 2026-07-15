using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HipChatServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly Kernel _kernel;
    public ChatController(Kernel kernel)
    {
        _kernel = kernel;
    }

    [HttpPost("askQuestion")]
    public async Task<IActionResult> AskQuestion([FromBody] string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return BadRequest("Prompt can not be empty!");
        }

        try
        {
            //retive chat completion service from kernel
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            //create chat history with previous promt.
            var chatHistory = new ChatHistory("AI Generated");
            chatHistory.AddUserMessage(question);

            //sending request to LM Studio
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, kernel: _kernel);

            return Ok(new { Response = response.Content});
        }

        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

    }
}
