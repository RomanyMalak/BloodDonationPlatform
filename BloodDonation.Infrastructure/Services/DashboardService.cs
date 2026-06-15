using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

//public class DashboardService : IDashboardService
//{
//    private readonly AppDbContext _context;

//    public DashboardService(AppDbContext context)
//    {
//        _context = context;
//    }

//    //public async Task<DashboardStatsDto> GetStatsAsync()
//    //{
//    //    var totalRequests = await _context.BloodRequests.CountAsync();

//    //    var activeRequests = await _context.BloodRequests
//    //        .CountAsync(r => r.Status == RequestStatus.Pending
//    //                      || r.Status == RequestStatus.Processing
//    //                      || r.Status == RequestStatus.NotificationsSent);

//    //    var totalDonors = await _context.Users
//    //        .CountAsync(u => u.Role == UserRole.Donor);

//    //    var totalDonations = await _context.DonationHistories.CountAsync();

//    //    var totalNotificationsSent = await _context.AiMatchingLogs
//    //        .SumAsync(l => (int?)l.NotificationsSent) ?? 0;

//    //    return new DashboardStatsDto
//    //    {
//    //        TotalRequests          = totalRequests,
//    //        ActiveRequests         = activeRequests,
//    //        TotalDonors            = totalDonors,
//    //        TotalDonations         = totalDonations,
//    //        TotalNotificationsSent = totalNotificationsSent,
//    //        TotalHospitals         = totalHospitals,
//    //        WaitingHospitals       = waitingHospitals,
//    //        ActiveHospitals        = activeHospitals,
//    //        RejectedHospitals      = rejectedHospitals
//    //    };
//    //}

//    public async Task<List<AiMatchingLogDto>> GetAiLogsAsync()
//    {
//        return await _context.AiMatchingLogs
//            .OrderByDescending(l => l.CreatedAt)
//            .Select(l => new AiMatchingLogDto
//            {
//                Id                       = l.Id,
//                BloodRequestId           = l.BloodRequestId,
//                PriorityResult           = l.PriorityResult.ToString(),
//                MatchedDonorsCount       = l.MatchedDonorsCount,
//                NotificationsSent        = l.NotificationsSent,
//                SearchRadiusKm           = l.SearchRadiusKm,
//                UsedCompatibleBloodTypes = l.UsedCompatibleBloodTypes,
//                PipelineSummary          = l.PipelineSummary,
//                CreatedAt                = l.CreatedAt
//            })
//            .ToListAsync();
//    }
//}
