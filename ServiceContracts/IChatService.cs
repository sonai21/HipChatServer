using HipChatServer.Models.DTOs;
using HipChatServer.Models.Entities;

namespace HipChatServer.ServiceContracts;

public interface IChatService
{
    Task<MessageResponseDTO?> ProcessUserMessageAsync(Guid id, string content);
}
