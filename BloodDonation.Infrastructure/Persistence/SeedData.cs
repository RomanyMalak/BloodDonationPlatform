using System;
using System.Collections.Generic;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Administrator",
            Email = "admin@blood.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            CreatedAt = now,
        };

        var hospitalUsers = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "57357 Hospital",
                Email = "hospital57357@blood.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.Hospital,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Ain Shams University Hospital",
                Email = "ainshams@blood.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.Hospital,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Cairo Specialized Hospital",
                Email = "cairo-specialized@blood.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.Hospital,
                CreatedAt = now,
            },
        };

        var hospitalProfiles = new List<Hospital>
        {
            new Hospital
            {
                Id = Guid.NewGuid(),
                UserId = hospitalUsers[0].Id,
                User = hospitalUsers[0],
                Name = "57357 Hospital",
                Government = "Cairo",
                City = "Cairo",
                AddressDetail = "El Sayeda Zeinab, Cairo",
                Hotline = "19057",
                Latitude = 30.0387,
                Longitude = 31.2624,
                IsActive = true,
                CreatedAt = now,
            },
            new Hospital
            {
                Id = Guid.NewGuid(),
                UserId = hospitalUsers[1].Id,
                User = hospitalUsers[1],
                Name = "Ain Shams University Hospital",
                Government = "Cairo",
                City = "Cairo",
                AddressDetail = "Abbassia, Cairo",
                Hotline = "0225123456",
                Latitude = 30.0633,
                Longitude = 31.2578,
                IsActive = true,
                CreatedAt = now,
            },
            new Hospital
            {
                Id = Guid.NewGuid(),
                UserId = hospitalUsers[2].Id,
                User = hospitalUsers[2],
                Name = "Cairo Specialized Hospital",
                Government = "Cairo",
                City = "Cairo",
                AddressDetail = "Heliopolis, Cairo",
                Hotline = "0222645678",
                Latitude = 30.0746,
                Longitude = 31.3304,
                IsActive = true,
                CreatedAt = now,
            },
        };

        hospitalUsers[0].Hospital = hospitalProfiles[0];
        hospitalUsers[1].Hospital = hospitalProfiles[1];
        hospitalUsers[2].Hospital = hospitalProfiles[2];

        var normalUsers = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Ahmed Mohamed",
                Email = "ahmed@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.User,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Sara Ali",
                Email = "sara@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.User,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Mahmoud Hassan",
                Email = "mahmoud@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.User,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Mona Adel",
                Email = "mona@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.User,
                CreatedAt = now,
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Omar Khaled",
                Email = "omar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = UserRole.User,
                CreatedAt = now,
            },
        };

        var bloodRequests = new List<BloodRequest>
        {
            new BloodRequest
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = normalUsers[0].Id,
                CreatedByUser = normalUsers[0],
                HospitalId = hospitalProfiles[0].Id,
                Hospital = hospitalProfiles[0],
                RequiredBloodType = BloodType.ONegative,
                UnitsNeeded = 3,
                Status = RequestStatus.PendingVerification,
                Urgency = RequestUrgency.Normal,
                ContactPhone = "19057",
                Notes = "Ahmed Mohamed's father needs O- blood at 57357 Hospital.",
                Latitude = hospitalProfiles[0].Latitude,
                Longitude = hospitalProfiles[0].Longitude,
                CreatedAt = now,
            },
            new BloodRequest
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = normalUsers[1].Id,
                CreatedByUser = normalUsers[1],
                HospitalId = hospitalProfiles[1].Id,
                Hospital = hospitalProfiles[1],
                RequiredBloodType = BloodType.APositive,
                UnitsNeeded = 2,
                Status = RequestStatus.Accepted,
                Urgency = RequestUrgency.Normal,
                ContactPhone = hospitalProfiles[1].Hotline,
                Notes = "Sara Ali's mother needs A+ blood at Ain Shams University Hospital.",
                Latitude = hospitalProfiles[1].Latitude,
                Longitude = hospitalProfiles[1].Longitude,
                CreatedAt = now,
            },
            new BloodRequest
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = normalUsers[2].Id,
                CreatedByUser = normalUsers[2],
                HospitalId = hospitalProfiles[2].Id,
                Hospital = hospitalProfiles[2],
                RequiredBloodType = BloodType.BPositive,
                UnitsNeeded = 1,
                Status = RequestStatus.Completed,
                Urgency = RequestUrgency.Normal,
                ContactPhone = hospitalProfiles[2].Hotline,
                Notes = "Mahmoud Hassan's brother needs B+ blood at Cairo Specialized Hospital.",
                Latitude = hospitalProfiles[2].Latitude,
                Longitude = hospitalProfiles[2].Longitude,
                CreatedAt = now,
            },
        };

        var notifications = new List<Notification>
        {
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = normalUsers[0].Id,
                BloodRequestId = bloodRequests[0].Id,
                Title = "Blood request created",
                Message = "Your request for O- blood at 57357 Hospital has been created.",
                Type = "Request",
                IsRead = false,
                CreatedAt = now,
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = normalUsers[1].Id,
                BloodRequestId = bloodRequests[1].Id,
                Title = "Blood request updated",
                Message = "Your A+ blood request at Ain Shams University Hospital is accepted.",
                Type = "Request",
                IsRead = false,
                CreatedAt = now,
            },
        };

        await context.Users.AddAsync(adminUser);
        await context.Users.AddRangeAsync(hospitalUsers);
        await context.Users.AddRangeAsync(normalUsers);
        await context.Hospitals.AddRangeAsync(hospitalProfiles);
        await context.BloodRequests.AddRangeAsync(bloodRequests);
        await context.Notifications.AddRangeAsync(notifications);

        await context.SaveChangesAsync();
    }
}
