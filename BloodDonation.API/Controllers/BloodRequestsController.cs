using BloodDonation.Application.Features.BloodRequests.Commands.AcceptBloodRequest;
using BloodDonation.Application.Features.BloodRequests.Commands.CancelBloodRequest;
using BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest;
using BloodDonation.Application.Features.BloodRequests.Queries.GetAvailableBloodRequests;
using BloodDonation.Application.Features.BloodRequests.Queries.GetBloodRequestDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/blood-requests")]
[Authorize]
public class BloodRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BloodRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/blood-requests
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBloodRequestCommand command,
        CancellationToken cancellationToken)
    {
        var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (patientId is null) return Unauthorized();

        var result = await _mediator.Send(
             command with { PatientId = Guid.Parse(patientId) },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET api/blood-requests/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBloodRequestDetailsQuery { BloodRequestId = id },
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    // GET api/blood-requests
    [HttpGet]
    public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAvailableBloodRequestsQuery(),
            cancellationToken);

        return Ok(result);
    }

    // POST api/blood-requests/{id}/accept
    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> Accept(
        Guid id,
        CancellationToken cancellationToken)
    {
        var donorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (donorId is null) return Unauthorized();

        var result = await _mediator.Send(
            new AcceptBloodRequestCommand
            {
                BloodRequestId = id,
                DonorId = Guid.Parse(donorId)
            },
            cancellationToken);

        return result
            ? Ok("Request accepted successfully.")
            : BadRequest("Unable to accept this request.");
    }

    // PUT api/blood-requests/{id}/cancel
    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (patientId is null) return Unauthorized();

        var result = await _mediator.Send(
            new CancelBloodRequestCommand
            {
                BloodRequestId = id,
                PatientId = Guid.Parse(patientId)
            },
            cancellationToken);

        return result
            ? Ok("Request cancelled successfully.")
            : BadRequest("Unable to cancel this request.");
    }
}