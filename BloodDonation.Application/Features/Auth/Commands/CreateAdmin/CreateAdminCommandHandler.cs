using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.Auth.Commands.CreateAdmin;
public sealed class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;
    public CreateAdminCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLower().Trim();

        var exists = await _dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (exists) return false;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Role = UserRole.Admin,
            IsAvailable = false,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(admin, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}