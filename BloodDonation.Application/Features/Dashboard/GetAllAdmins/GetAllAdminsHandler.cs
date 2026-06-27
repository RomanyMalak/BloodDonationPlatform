using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Features.Dashboard.GetAllAdmins
{
    public sealed class GetAllAdminsHandler
    : IRequestHandler<GetAllAdminsQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllAdminsHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(
            GetAllAdminsQuery request,
            CancellationToken cancellationToken)
        {

            return await _context.Users
                .Where(x => x.Role == UserRole.Admin)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    Email = x.Email
                })
                .ToListAsync(cancellationToken);

        }
    }
}
