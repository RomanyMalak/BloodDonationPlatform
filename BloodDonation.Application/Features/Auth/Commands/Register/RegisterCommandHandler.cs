using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public RegisterCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var existingemail = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (existingemail)
            {
                throw new Exception("Email already exists");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);



            var user = new User
            {

                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Phone = request.Phone,
                Age = request.Age,
                BloodType = request.BloodType,
                Role = request.Role,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            string token = _jwtTokenGenerator.GenerateToken(user);
            return new RegisterResponseDto(user.Id, user.FullName, token);
        }
    }
}
