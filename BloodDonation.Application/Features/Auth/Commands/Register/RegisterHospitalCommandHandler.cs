using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Register
{
    public class RegisterHospitalCommandHandler : IRequestHandler<RegisterHospitalCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IFileService _fileService; // 1️⃣ عملنا Inject للخدمة المركزية هنا

        public RegisterHospitalCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator,IFileService fileService)       {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _fileService = fileService;
        }
        public async Task<Guid> Handle(RegisterHospitalCommand request, CancellationToken cancellationToken)
        {
            
                var emailExists = _context.Users.Any(u => u.Email == request.Email);
                if (emailExists)
                {
                    throw new Exception("Email already exists.");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            // 2️⃣ رفع الملف باستخدام الخدمة المركزية والحصول على المسار الذي سيتم تخزينه في قاعدة البيانات
            string savedDbPath = await _fileService.UploadFileAsync(request.LicenseDocumentPath, "licenses", cancellationToken);

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
                LicenseDocumentPath = savedDbPath,// 3️⃣ بنخزن المسار اللي رجع من الخدمة المركزية في قاعدة البيانات
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
