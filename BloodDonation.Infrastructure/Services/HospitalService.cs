//using BloodDonation.Application.DTOs.Hospital;
//using BloodDonation.Application.Interfaces;
//using BloodDonation.Domain.Entities;
//using BloodDonation.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace BloodDonation.Infrastructure.Services;

//public class HospitalService : IHospitalService
//{
//    private readonly AppDbContext _context;

//    public HospitalService(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<HospitalDto>> GetAllAsync()
//    {
//        return await _context.Hospitals
//            .OrderByDescending(h => h.CreatedAt)
//            .Select(h => MapToDto(h))
//            .ToListAsync();
//    }

//    public async Task<List<HospitalDto>> GetByStatusAsync(HospitalStatus status)
//    {
//        return await _context.Hospitals
//            .Where(h => h.Status == status)
//            .OrderByDescending(h => h.CreatedAt)
//            .Select(h => MapToDto(h))
//            .ToListAsync();
//    }

//    public async Task<HospitalDto?> GetByIdAsync(Guid id)
//    {
//        var hospital = await _context.Hospitals
//            .Include(h => h.BloodRequests)
//            .FirstOrDefaultAsync(h => h.Id == id);

//        if (hospital == null) return null;

//        var dto = MapToDto(hospital);
//        dto.TotalBloodRequests = hospital.BloodRequests.Count;
//        return dto;
//    }

//    public async Task<HospitalDto> ApproveAsync(Guid id)
//    {
//        var hospital = await _context.Hospitals.FindAsync(id)
//            ?? throw new Exception($"Hospital with id {id} not found.");

//        if (hospital.Status == HospitalStatus.Active)
//            throw new Exception("Hospital is already approved.");

//        hospital.Status = HospitalStatus.Active;
//        hospital.ReviewedAt = DateTime.UtcNow;
//        hospital.RejectionReason = null;

//        await _context.SaveChangesAsync();
//        return MapToDto(hospital);
//    }

//    public async Task<HospitalDto> RejectAsync(Guid id, string? reason)
//    {
//        var hospital = await _context.Hospitals.FindAsync(id)
//            ?? throw new Exception($"Hospital with id {id} not found.");

//        if (hospital.Status == HospitalStatus.Rejected)
//            throw new Exception("Hospital is already rejected.");

//        hospital.Status = HospitalStatus.Rejected;
//        hospital.ReviewedAt = DateTime.UtcNow;
//        hospital.RejectionReason = reason;

//        await _context.SaveChangesAsync();
//        return MapToDto(hospital);
//    }

//    private static HospitalDto MapToDto(Hospital h) => new HospitalDto
//    {
//        Id = h.Id,
//        Name = h.Name,
//        City = h.City,
//        Address = h.Address,
//        Latitude = h.Latitude,
//        Longitude = h.Longitude,
//        IsKnown = h.IsKnown,
//        Status = h.Status,
//        CreatedAt = h.CreatedAt,
//        ReviewedAt = h.ReviewedAt,
//        RejectionReason = h.RejectionReason
//    };
//}
