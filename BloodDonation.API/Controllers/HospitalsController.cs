using BloodDonation.Application.Features.Hospitals.Commands.ApproveBloodRequest;
using BloodDonation.Application.Features.Hospitals.Commands.RejectBloodRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public HospitalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("requests/{id}/approve")]
        public async Task<IActionResult> Approve(Guid id,ApproveBloodRequestCommand command)
        {
            command = command with
            {
                BloodRequestId = id
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("requests/{id}/reject")]
        public async Task<IActionResult> Reject( Guid id,RejectBloodRequestCommand command)
        {
            command = command with
            {
                BloodRequestId = id
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}
