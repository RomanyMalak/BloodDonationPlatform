using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // الشات بوت متاح لأي زائر للموقع، حتى قبل تسجيل الدخول
public class ChatController : ControllerBase
{
    private readonly IChatAssistantService _chatAssistantService;

    public ChatController(IChatAssistantService chatAssistantService)
    {
        _chatAssistantService = chatAssistantService;
    }

    // POST /api/chat
    // الـ widget في الفرونت إند يستدعي الـ endpoint ده مباشرة بأي سؤال من الزائر.
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ChatResponseDto { Success = false, Error = "الرسالة مطلوبة." });
        }

        var result = await _chatAssistantService.SendMessageAsync(request.Message, cancellationToken);

        if (!result.Success)
        {
            return StatusCode(502, result); // 502 يعني الخدمة الخارجية (OpenPipe) فشلت
        }

        return Ok(result);
    }
}
