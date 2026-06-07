using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public LoginCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        // "email": "BloodDonationAdmin@gmail.com",
  //"password": "ITI2026BloodDonationAdmin",
        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                 .Include(u => u.Hospital)
                 .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // التحقق من صحة كلمة المرور
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

  
            // لو اليوزر مستشفى، والبروفايل بتاعها مش موجود أو لسه مش متفعل (IsActive == false)، بنمنع الـ Login فوراً
            if (user.Role == UserRole.Hospital)
            {
                if (user.Hospital == null || !user.Hospital.IsActive)
                {
                    throw new UnauthorizedAccessException("عذراً، حساب المستشفى لم يتم تفعيله من قبل الأدمن حتى الآن.");
                }
            }

            // توليد التوكن (التوكن تلقائياً هياخد الـ Role الجديد)
            var token = _jwtTokenGenerator.GenerateToken(user);

            // إرجاع البيانات للـ Angular (لاحظي استبدال Task.FromResult لأن الميثود أصلاً Async بـ await للـ FirstOrDefault فمش محتاجينها)
            return new LoginResponseDto(
                user.Id,
                user.FullName ?? user.Hospital?.Name ?? "Hospital User", // هندلة لو الـ FullName بنل للمستشفى يرجع اسمها الرسمي
                user.Email,
                user.Role.ToString(),
                token
            );
        }
    }
}

