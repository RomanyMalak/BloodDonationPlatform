using BloodDonation.Application.DTOs.Hospital;
using BloodDonation.Application.Features.Hospitals.Commands.ApproveBloodRequest;
using BloodDonation.Application.Features.Hospitals.Commands.RejectBloodRequest;
using BloodDonation.Application.Features.Hospitals.Queries.GetPendingRequests;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/hospitals")]

public class HospitalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _dbContext;
    private readonly IHospitalService _hospitalService;

    public HospitalsController(IMediator mediator, IApplicationDbContext dbContext,IHospitalService hospitalService)
    {
        _mediator = mediator;
        _dbContext = dbContext;
        _hospitalService = hospitalService;
    }
    private async Task<Guid?> GetHospitalIdFromJwt(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return null;

        var hospital = await _dbContext.Hospitals
            .FirstOrDefaultAsync(h => h.UserId == Guid.Parse(userId), ct);

        return hospital?.Id;
    }
    // ══════════════════════════════════════════════════════
    //  ADMIN ENDPOINTS  [Authorize(Roles = "Admin")]
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// GET api/hospitals
    /// جيب كل المستشفيات (أدمن بس)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _hospitalService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// GET api/hospitals/waiting
    /// جيب المستشفيات اللي لسه بتستنى موافقة
    /// </summary>
    [HttpGet("waiting")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetWaiting()
    {
        var result = await _hospitalService.GetByStatusAsync(HospitalStatus.Waiting);
        return Ok(result);
    }

    /// <summary>
    /// GET api/hospitals/active
    /// جيب المستشفيات الفعّالة
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _hospitalService.GetByStatusAsync(HospitalStatus.Active);
        return Ok(result);
    }

    /// <summary>
    /// GET api/hospitals/rejected
    /// جيب المستشفيات المرفوضة
    /// </summary>
    [HttpGet("rejected")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRejected()
    {
        var result = await _hospitalService.GetByStatusAsync(HospitalStatus.Rejected);
        return Ok(result);
    }

    /// <summary>
    /// GET api/hospitals/{id}
    /// جيب مستشفى واحد بالـ ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _hospitalService.GetByIdAsync(id);
        if (result is null)
            return NotFound(new { message = $"Hospital {id} not found." });

        return Ok(result);
    }

    /// <summary>
    /// POST api/hospitals/{id}/approve
    /// الأدمن يوافق على تسجيل مستشفى
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveHospital(Guid id)
    {
        try
        {
            var result = await _hospitalService.ApproveAsync(id);
            return Ok(new { message = "Hospital approved successfully.", hospital = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST api/hospitals/{id}/reject
    /// الأدمن يرفض تسجيل مستشفى
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectHospital(Guid id, [FromBody] RejectHospitalDto dto)
    {
        try
        {
            var result = await _hospitalService.RejectAsync(id, dto?.Reason);
            return Ok(new { message = "Hospital rejected.", hospital = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    // ══════════════════════════════════════════════════════
    //  HOSPITAL ENDPOINTS  [Authorize(Roles = "Hospital")]
    // ══════════════════════════════════════════════════════
    [HttpPut("requests/{id}/approveBloodRequest")]
    [Authorize(Roles = "Hospital")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        var hospitalId = await GetHospitalIdFromJwt(ct);
        if (hospitalId is null) return Unauthorized();

        var result = await _mediator.Send(new ApproveBloodRequestCommand
        {
            BloodRequestId = id,
            HospitalId = hospitalId.Value
        }, ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("requests/{id}/rejectBloodRequest")]
    [Authorize(Roles = "Hospital")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectBloodRequestCommand body, CancellationToken ct)
    {
        var hospitalId = await GetHospitalIdFromJwt(ct);
        if (hospitalId is null) return Unauthorized();

        var result = await _mediator.Send(body with
        {
            BloodRequestId = id,
            HospitalId = hospitalId.Value
        }, ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("requests/pendingBloodRequest")]
    [Authorize(Roles = "Hospital")]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var hospitalId = await GetHospitalIdFromJwt(ct);
        if (hospitalId is null) return Unauthorized();

        var result = await _mediator.Send(
            new GetPendingRequestsQuery { HospitalId = hospitalId.Value }, ct);

        return Ok(result);
    }
}
