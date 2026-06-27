using BloodDonation.Application.Features.Donors.Query;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;
    private readonly IDonorMatchingService _donorMatchingService;
    private readonly IMediator _mediator;

    public DonorsController(IDonorService donorService , IDonorMatchingService donorMatchingService, IMediator mediator)
    {
        _donorService = donorService;
        _donorMatchingService = donorMatchingService;
        _mediator = mediator;
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
   
   
   
    //[HttpGet("available")]
    //public async Task<IActionResult> GetAvailableDonors()
    //{
    //    var donors = await _donorService.GetAvailableDonorsAsync();
    //    return Ok(donors);
    //}
    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetAvailableBloodTypes(Guid requestId)
    {
        var result = await _mediator.Send(
            new GetAvailableBloodTypesQuery(requestId));

        return Ok(result);
    }

    [HttpGet("{bloodRequestId}/matched-donors/count")]
    public async Task<IActionResult> GetMatchedDonorsCount(
    Guid bloodRequestId,
    CancellationToken cancellationToken)
    {
        var count = await _donorMatchingService.GetMatchedDonorsCountAsync(
            bloodRequestId,
            cancellationToken);

        return Ok(new
        {
            matchedDonorsCount = count
        });
    }

    private Guid? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null) return null;
        if (Guid.TryParse(idClaim, out var id)) return id;
        return null;
    }
}
