using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public class CreateBloodRequestValidator : AbstractValidator<CreateBloodRequestCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public CreateBloodRequestValidator()
        {
            RuleFor(x => x.PatientName)
                .NotEmpty().WithMessage("Patient name is required.")
                .MaximumLength(50).WithMessage("Patient name must not exceed 50 characters.");

            RuleFor(x => x.PatientAge)
                .InclusiveBetween(0, 120)
                .When(x => x.PatientAge.HasValue)
                .WithMessage("Patient age must be between 0 and 120.");

            RuleFor(x => x.RequiredBloodType)
                .NotNull().WithMessage("Required blood type is required.")
                .IsInEnum().WithMessage("Required blood type is invalid.");

            RuleFor(x => x.Urgency)
                .IsInEnum().WithMessage("Urgency level is invalid.");

            RuleFor(x => x)
                   .Must(x => x.HospitalId.HasValue ||
                   !string.IsNullOrWhiteSpace(x.CustomHospitalName))
                   .WithMessage("Provide either a registered HospitalId or a CustomHospitalName.");

            RuleFor(x => x)
                  .Must(x => !(x.HospitalId.HasValue &&
                  !string.IsNullOrWhiteSpace(x.CustomHospitalName)))
                  .WithMessage("Provide either HospitalId or CustomHospitalName, not both.");


            RuleFor(x => x.CustomHospitalName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.CustomHospitalName))
                .WithMessage("Custom hospital name must not exceed 100 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be a valid coordinate.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be a valid coordinate.");


            // Document required for custom hospital
            RuleFor(x => x.MedicalDocumentUrl)
                .NotNull()
                .When(x => !x.HospitalId.HasValue &&
                           !string.IsNullOrWhiteSpace(x.CustomHospitalName))
                .WithMessage("Medical document is required for custom hospitals.");


            // Document not allowed for registered hospital
            RuleFor(x => x.MedicalDocumentUrl)
                .Empty()
                .When(x => x.HospitalId.HasValue)
                .WithMessage("Medical document is not allowed for registered hospitals.");


            // File validation
            RuleFor(x => x.MedicalDocumentUrl)
                .Must(HasValidSize)
                .WithMessage("File size must not exceed 10 MB.")
                .Must(HasValidExtension)
                .WithMessage("Only JPG, PNG, or PDF files are allowed.")
                .When(x => x.MedicalDocumentUrl != null);

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.")
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Contact phone must be a valid Egyptian phone number.");

            RuleFor(x => x.UnitsNeeded)
                .GreaterThan(0).WithMessage("Units needed must be greater than 0.")
                .LessThanOrEqualTo(20).WithMessage("Units needed must not exceed 20.");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.ExpiresAt.HasValue)
                .WithMessage("Expiry date must be in the future.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Notes))
                .WithMessage("Notes must not exceed 500 characters.");
        }

        private static bool HasValidSize(IFormFile? file)
            => file is null || file.Length <= MaxFileSizeBytes;

        private static bool HasValidExtension(IFormFile? file)
        {
            if (file is null) return true;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }
}