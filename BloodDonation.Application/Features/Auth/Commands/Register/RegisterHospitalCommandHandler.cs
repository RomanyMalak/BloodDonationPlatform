using BloodDonation.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    public class RegisterHospitalCommandHandler : IRequestHandler<RegisterHospitalCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterHospitalCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<Guid> Handle(RegisterHospitalCommand request, CancellationToken cancellationToken)
        {
            
                var emailExists = _context.Users.Any(u => u.Email == request.Email);
                if (emailExists)
                {
                    throw new Exception("Email already exists.");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = UserRole.Hospital,
             
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
        
            var hospital = new Hospital
            {
                Id = Guid.NewGuid(),
                Name = request.HospitalName,
                Government = request.Government,
                City = request.City,
                AddressDetail = request.AddressDetail,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Hotline = request.Hotline,
                LicenseDocumentPath = request.LicenseDocumentPath,
                UserId = user.Id,
                IsActive = false,
                Status = HospitalStatus.Waiting

            };

            _context.Hospitals.Add(hospital);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // 💡 حطي النقطة الحمراء (Breakpoint) على السطر اللي تحت ده بالظبط 👇
                var sqlError = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlError);
            }

            return  hospital.Id;
        }
    }
}
