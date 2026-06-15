using BloodDonation.Application.DTOs.Hospital;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface IHospitalService
{
    Task<List<HospitalDto>> GetAllAsync();
    Task<List<HospitalDto>> GetByStatusAsync(HospitalStatus status);
    Task<HospitalDto?> GetByIdAsync(Guid id);
    Task<HospitalDto> ApproveAsync(Guid id);
    Task<HospitalDto> RejectAsync(Guid id, string? reason);
}
