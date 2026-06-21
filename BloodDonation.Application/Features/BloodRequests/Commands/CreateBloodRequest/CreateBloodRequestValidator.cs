using FluentValidation;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public class CreateBloodRequestValidator : AbstractValidator<CreateBloodRequestCommand>
    {
        public CreateBloodRequestValidator()
        {
            RuleFor(x => x.PatientName)
                .NotEmpty().WithMessage("Patient name is required.")
                .MaximumLength(50).WithMessage("Patient name must not exceed 50 characters.");

            RuleFor(x => x.UnitsNeeded)
                .GreaterThan(0).WithMessage("Units needed must be greater than 0.");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.")
                .MaximumLength(11).WithMessage("Contact phone must not exceed 11 characters.");

            RuleFor(x => x.MedicalDocumentUrl)
                .NotEmpty().WithMessage("Medical document is required.")
                .Must(file => file == null || file.Length <= 10 * 1024 * 1024)
                .WithMessage("File size must not exceed 10 MB.");

            RuleFor(x => x.ExpiresAt)
                .NotEmpty().WithMessage("Expiry date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");
        }
    }
}