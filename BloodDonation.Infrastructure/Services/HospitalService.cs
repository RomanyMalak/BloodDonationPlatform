using BloodDonation.Application.DTOs.Hospital;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public class HospitalService : IHospitalService
{
    private readonly AppDbContext _context;

    public HospitalService(AppDbContext context)
    {
        _context = context;
    }

    // جيب كل المستشفيات
    public async Task<List<HospitalDto>> GetAllAsync()
    {
        return await _context.Hospitals
            .Include(h => h.BloodRequests)
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    // جيب المستشفيات بناءً على الـ Status
    public async Task<List<HospitalDto>> GetByStatusAsync(HospitalStatus status)
    {
        return await _context.Hospitals
            .Include(h => h.BloodRequests)
            .Where(h => h.Status == status)
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    // جيب مستشفى واحد بالـ ID
    public async Task<HospitalDto?> GetByIdAsync(Guid id)
    {
        var hospital = await _context.Hospitals
            .Include(h => h.BloodRequests)
            .FirstOrDefaultAsync(h => h.Id == id);

        return hospital == null ? null : MapToDto(hospital);
    }

    // الأدمن يوافق على مستشفى
    public async Task<HospitalDto> ApproveAsync(Guid id)
    {
        var hospital = await _context.Hospitals.FindAsync(id)
            ?? throw new Exception($"Hospital {id} not found.");

        if (hospital.Status == HospitalStatus.Active)
            throw new Exception("Hospital is already approved.");

        hospital.Status = HospitalStatus.Active;
        hospital.IsActive = true;
        hospital.ReviewedAt = DateTime.UtcNow;
        hospital.RejectionReason = null;

        await _context.SaveChangesAsync();
        return MapToDto(hospital);
    }

    // الأدمن يرفض مستشفى
    public async Task<HospitalDto> RejectAsync(Guid id, string? reason)
    {
        var hospital = await _context.Hospitals.FindAsync(id)
            ?? throw new Exception($"Hospital {id} not found.");

        if (hospital.Status == HospitalStatus.Rejected)
            throw new Exception("Hospital is already rejected.");

        hospital.Status = HospitalStatus.Rejected;
        hospital.IsActive = false;
        hospital.ReviewedAt = DateTime.UtcNow;
        hospital.RejectionReason = reason;

        await _context.SaveChangesAsync();
        return MapToDto(hospital);
    }

    private static HospitalDto MapToDto(Hospital h) => new HospitalDto
    {
        Id = h.Id,
        Name = h.Name,
        City = h.City,
        Address = h.AddressDetail,
        Latitude = h.Latitude,
        Longitude = h.Longitude,
        IsKnown = h.IsActive,
        Status = (HospitalStatus)h.Status,
        CreatedAt = h.CreatedAt,
        ReviewedAt = h.ReviewedAt,
        RejectionReason = h.RejectionReason,
        TotalBloodRequests = h.BloodRequests?.Count ?? 0
    };
}
