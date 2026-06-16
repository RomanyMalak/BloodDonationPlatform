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

    public async Task<List<HospitalDto>> GetAllAsync()
    {
        return await _context.Hospitals
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    public async Task<List<HospitalDto>> GetByStatusAsync(HospitalStatus status)
    {
        // Domain `Hospital` doesn't have a direct `Status` property in the current model.
        // Infer the requested status from existing fields (IsActive). For Rejected state
        // we don't have a persistent flag in the entity, so return empty for Rejected.
        if (status == HospitalStatus.Rejected)
        {
            return new List<HospitalDto>();
        }

        return await _context.Hospitals
            .Where(h => status == HospitalStatus.Active ? h.IsActive : !h.IsActive)
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    public async Task<HospitalDto?> GetByIdAsync(Guid id)
    {
        var hospital = await _context.Hospitals
            .Include(h => h.BloodRequests)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hospital == null) return null;

        var dto = MapToDto(hospital);
        dto.TotalBloodRequests = hospital.BloodRequests.Count;
        return dto;
    }

    public async Task<HospitalDto> ApproveAsync(Guid id)
    {
        var hospital = await _context.Hospitals.FindAsync(id)
            ?? throw new Exception($"Hospital with id {id} not found.");

        if (hospital.IsActive)
            throw new Exception("Hospital is already approved.");

        hospital.IsActive = true;
        // ReviewedAt / RejectionReason not present on current domain model - cannot set.

        await _context.SaveChangesAsync();
        return MapToDto(hospital);
    }

    public async Task<HospitalDto> RejectAsync(Guid id, string? reason)
    {
        var hospital = await _context.Hospitals.FindAsync(id)
            ?? throw new Exception($"Hospital with id {id} not found.");

        if (!hospital.IsActive && string.IsNullOrEmpty(reason))
            throw new Exception("Hospital is already rejected or no reason provided.");

        hospital.IsActive = false;
        // We do not have persisted ReviewedAt / RejectionReason on Hospital entity in domain.

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
        // Map existing `IsActive` to DTO's `IsKnown` and infer Status from it.
        IsKnown = h.IsActive,
        Status = h.IsActive ? HospitalStatus.Active : HospitalStatus.Waiting,
        CreatedAt = h.CreatedAt,
        ReviewedAt = null,
        RejectionReason = null
    };
}
