using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

/// <summary>
/// عقد خدمة المساعد الذكي (Chat Assistant) المتخصص في التبرع بالدم.
/// التنفيذ الفعلي بيكلم OpenPipe عن طريق HTTP (انظر Infrastructure/Services/ChatAssistantService.cs).
/// </summary>
public interface IChatAssistantService
{
    Task<ChatResponseDto> SendMessageAsync(string userMessage, CancellationToken cancellationToken);
}
