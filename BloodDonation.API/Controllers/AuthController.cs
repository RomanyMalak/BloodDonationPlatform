using BloodDonation.Application.Features.Auth.Commands.Login;
using BloodDonation.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("register/User")]
    public async Task<IActionResult> Register([FromBody]RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("register/Hospital")]
    public async Task<IActionResult> Register([FromForm] RegisterHospitalCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
