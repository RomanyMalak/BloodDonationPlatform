using BloodDonation.Application.Interfaces;
using BloodDonation.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly IMedicalValidatorAgent _medicalValidatorAgent;
        private readonly IDonorQueryService _donorQueryService;
        private readonly IDonorMatchingService _donorMatchingService;

        public testController(IMedicalValidatorAgent medicalValidatorAgent, IDonorQueryService donorQueryService, IDonorMatchingService donorMatchingService)
        {
            _medicalValidatorAgent = medicalValidatorAgent;
            _donorQueryService = donorQueryService;
            _donorMatchingService = donorMatchingService;
        }

        [HttpGet]
        public async Task<IActionResult> Test(CancellationToken cancellationToken)
        {
            var result = await _medicalValidatorAgent.GetCompatibleBloodTypesAsync(
                "APositive",
                new List<string>
                {
                "APositive",
                "ANegative",
                "OPositive",
                "BPositive"
                },
                cancellationToken);

            return Ok(result);
        }
        [HttpGet("donors")]
        public async Task<IActionResult> GetDonors(
    CancellationToken cancellationToken)
        {
            var result =
                await _donorQueryService.GetAvailableDonorsAsync(
                    cancellationToken);

            return Ok(result);
        }
        [HttpGet("ai")]
        public async Task<IActionResult> TestAi(
    CancellationToken cancellationToken)
        {
            var donors =
                await _donorQueryService
                    .GetAvailableDonorsAsync(cancellationToken);

            var availableBloodTypes =
                donors
                .Select(x => x.BloodType)
                .Distinct()
                .ToList();

            var compatibleBloodTypes =
                await _medicalValidatorAgent.GetCompatibleBloodTypesAsync(
                    "APositive",
                    availableBloodTypes,
                    cancellationToken);

            var matchedDonors = donors
    .Where(d =>
        compatibleBloodTypes.Contains(d.BloodType))
    .ToList();

            return Ok(matchedDonors);
        }


        [HttpGet("blood-types/{requestId}")]
        public async Task<IActionResult> GetBloodTypes(
        Guid requestId,
        CancellationToken cancellationToken)
        {
            var result = await _donorMatchingService
                .GetAvailableBloodTypesAsync(
                    requestId,
                    cancellationToken);

            return Ok(result);
        }
        [HttpGet("matched-donors/{requestId}")]
        public async Task<IActionResult> GetMatchedDonors(
    Guid requestId,
    CancellationToken cancellationToken)
        {
            var result =
                await _donorMatchingService.GetMatchedDonorsAsync(
                    requestId,
                    cancellationToken);

            return Ok(result);
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var result = await _medicalValidatorAgent.GetCompatibleBloodTypesAsync(
                "A+",
                new List<string> { "A+", "O+", "B+" },
                CancellationToken.None);

            return Ok(result);
        }
    }
}
