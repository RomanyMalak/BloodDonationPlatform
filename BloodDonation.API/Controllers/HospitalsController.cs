using BloodDonation.Application.Features.Hospitals.Commands.ApproveBloodRequest;
using BloodDonation.Application.Features.Hospitals.Commands.RejectBloodRequest;
using BloodDonation.Application.Features.Hospitals.Queries.GetPendingRequests;
using BloodDonation.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/hospitals")]
[Authorize(Roles = "Hospital")]
public class HospitalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _dbContext;

    public HospitalsController(IMediator mediator, IApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext;
    }

    private async Task<Guid?> GetHospitalIdFromJwt(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return null;

        var hospital = await _dbContext.Hospitals
            .FirstOrDefaultAsync(h => h.UserId == Guid.Parse(userId), ct);

        return hospital?.Id;
    }

    [HttpPut("requests/{id}/approveBloodRequest")]
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
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var hospitalId = await GetHospitalIdFromJwt(ct);
        if (hospitalId is null) return Unauthorized();

        var result = await _mediator.Send(
            new GetPendingRequestsQuery { HospitalId = hospitalId.Value }, ct);

        return Ok(result);
    }
}