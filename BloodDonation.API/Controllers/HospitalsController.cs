using BloodDonation.Application.Features.BloodRequests.Commands.ApproveBloodRequest;
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

    }
}
