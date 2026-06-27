namespace BloodDonation.Application.DTOs;

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}

public class ChatResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
