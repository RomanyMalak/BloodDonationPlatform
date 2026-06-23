using MediatR;
namespace BloodDonation.Application.Features.Auth.Commands.CreateAdmin;
public sealed record CreateAdminCommand : IRequest<bool>
{
    public string FullName { get; init; } 
    public string Email { get; init; } 
    public string Password { get; init; } 
    public string? Phone { get; init; }
}