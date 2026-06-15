using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Persistence
{
    internal class DbInitializer
    {
        public static async Task SeedAdminAsync(DbContext context)
        {
            // 1. التأكد إن الداتا بيز مش فيها أدمن مسجل مسبقاً
            var adminExists = await context.Set<User>().AnyAsync(u => u.Role == UserRole.Admin);

            if (!adminExists)
            {
                // 2. تشفير باسوورد الأدمن الافتراضي
                string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("ITI2026BloodDonationAdmin");

                // 3. إنشاء  الأدمن
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Main Administrator",
                    Email = "admin@blooddonation.com",
                    PasswordHash = defaultPasswordHash,
                    Role = UserRole.Admin, 
                    CreatedAt = DateTime.UtcNow
                };

                context.Set<User>().Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
