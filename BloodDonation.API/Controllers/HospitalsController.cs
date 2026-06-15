using BloodDonation.Application.DTOs.Hospital;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]  // Uncomment when Auth middleware is fully wired
public class HospitalsController : ControllerBase
{
    private readonly IHospitalService _hospitalService;

    public HospitalsController(IHospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }

    /// <summary>
    /// GET /api/hospitals
    /// Returns all hospitals (Admin Dashboard)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hospitals = await _hospitalService.GetAllAsync();
        return Ok(hospitals);
    }

    /// <summary>
    /// GET /api/hospitals/waiting
    /// Returns hospitals pending approval
    /// </summary>
    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaiting()
    {
        var hospitals = await _hospitalService.GetByStatusAsync(HospitalStatus.Waiting);
        return Ok(hospitals);
    }

    /// <summary>
    /// GET /api/hospitals/active
    /// Returns approved/active hospitals
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var hospitals = await _hospitalService.GetByStatusAsync(HospitalStatus.Active);
        return Ok(hospitals);
    }

    /// <summary>
    /// GET /api/hospitals/rejected
    /// Returns rejected hospitals
    /// </summary>
    [HttpGet("rejected")]
    public async Task<IActionResult> GetRejected()
    {
        var hospitals = await _hospitalService.GetByStatusAsync(HospitalStatus.Rejected);
        return Ok(hospitals);
    }

    /// <summary>
    /// GET /api/hospitals/{id}
    /// Returns a single hospital details
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var hospital = await _hospitalService.GetByIdAsync(id);
        if (hospital == null)
            return NotFound(new { message = $"Hospital {id} not found." });

        return Ok(hospital);
    }

    /// <summary>
    /// POST /api/hospitals/{id}/approve
    /// Admin approves a hospital
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            var result = await _hospitalService.ApproveAsync(id);
            return Ok(new { message = "Hospital approved successfully.", hospital = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/hospitals/{id}/reject
    /// Admin rejects a hospital with optional reason
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectHospitalDto? dto)
    {
        try
        {
            var result = await _hospitalService.RejectAsync(id, dto?.Reason);
            return Ok(new { message = "Hospital rejected.", hospital = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
