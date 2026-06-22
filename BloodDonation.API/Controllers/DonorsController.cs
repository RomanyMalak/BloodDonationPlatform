using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;

    public DonorsController(IDonorService donorService)
    {
        _donorService = donorService;
    }

    // GET /api/donors/nearby-requests
    // الطلبات القريبة من المتبرع
    [HttpGet("nearby-requests")]
    public async Task<IActionResult> GetNearbyRequests()
    {
        var donorId = GetCurrentUserId();
        if (donorId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var requests = await _donorService.GetNearbyRequestsAsync(donorId.Value);
        return Ok(requests);
    }

    // PUT /api/donors/availability
    // تحديث حالة توافر المتبرع
    [HttpPut("availability")]
    public async Task<IActionResult> UpdateAvailability([FromBody] bool isAvailable)
    {
        var donorId = GetCurrentUserId();
        if (donorId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        await _donorService.UpdateAvailabilityAsync(donorId.Value, isAvailable);
        return Ok(new { message = "تم تحديث حالة التوافر بنجاح", isAvailable });
    }

    // GET /api/donors/my-history
    // سجل تبرعات المتبرع
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyDonationHistory()
    {
        var donorId = GetCurrentUserId();
        if (donorId == null)
            return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

        var history = await _donorService.GetDonationHistoryAsync(donorId.Value);
        return Ok(history);
    }
    // GET /api/donors/eligible?bloodType=A+
    [AllowAnonymous]
    [HttpGet("eligible")]
    public async Task<IActionResult> GetEligibleDonors([FromQuery] BloodType bloodType)
    {
        var donors = await _donorService.GetEligibleDonorsAsync(bloodType);
        return Ok(donors);
    }

    private Guid? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null) return null;
        if (Guid.TryParse(idClaim, out var id)) return id;
        return null;
    }
}
