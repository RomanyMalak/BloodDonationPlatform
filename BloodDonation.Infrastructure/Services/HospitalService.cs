using BloodDonation.Application.DTOs.Hospital;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Infrastructure.Services;

public class HospitalService : IHospitalService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HospitalService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // جيب كل المستشفيات
    public async Task<List<HospitalDto>> GetAllAsync()
    {
        var hospitals = await _context.Hospitals
            .Include(h => h.BloodRequests)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

        return hospitals.Select(MapToDto).ToList();
    }

    // جيب المستشفيات بناءً على الـ Status
    public async Task<List<HospitalDto>> GetByStatusAsync(HospitalStatus status)
    {
        var hospitals = await _context.Hospitals
            .Include(h => h.BloodRequests)
            .Where(h => h.Status == status)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

        return hospitals.Select(MapToDto).ToList();
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

    private HospitalDto MapToDto(Hospital h) => new HospitalDto
    {
        Id = h.Id,
        Name = h.Name,
        City = h.City,
        Address = h.AddressDetail,
        Latitude = h.Latitude,
        Longitude = h.Longitude,
        IsKnown = h.IsActive,
        Status = h.Status ?? HospitalStatus.Waiting,
        CreatedAt = h.CreatedAt,
        ReviewedAt = h.ReviewedAt,
        RejectionReason = h.RejectionReason,
        LicenseDocumentUrl = BuildLicenseDocumentUrl(h.LicenseDocumentPath),
        TotalBloodRequests = h.BloodRequests?.Count ?? 0
    };

    private string? BuildLicenseDocumentUrl(string? licenseDocumentPath)
    {
        if (string.IsNullOrWhiteSpace(licenseDocumentPath))
            return null;

        if (Uri.TryCreate(licenseDocumentPath, UriKind.Absolute, out _))
            return licenseDocumentPath;

        var request = _httpContextAccessor.HttpContext?.Request;
        var relativePath = licenseDocumentPath.TrimStart('/');

        if (request is null)
            return "/" + relativePath;

        return $"{request.Scheme}://{request.Host}/{relativePath}";
    }
}
