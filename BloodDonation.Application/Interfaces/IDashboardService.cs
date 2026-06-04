using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
    Task<List<AiMatchingLogDto>> GetAiLogsAsync();
}
