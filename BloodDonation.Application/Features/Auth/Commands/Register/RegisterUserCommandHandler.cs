using BloodDonation.Application.Exceptions;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public RegisterUserCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<RegisterResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.ToLower().Trim();
            var existingemail = await _context.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
            if (existingemail)
            {
                throw new ConflictException("Email already exists");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);



            var user = new User
            {

                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = normalizedEmail,
                PasswordHash = hashedPassword,
                Phone = request.Phone,
                Age = request.Age,
                BloodType = request.BloodType,
                Role = UserRole.User,
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
