using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.Features.BloodRequests.Commands.CreateBloodRequest
{
    public class CreateBloodRequestValidator
     : AbstractValidator<CreateBloodRequestCommand>
    {
        public CreateBloodRequestValidator()
        {
            RuleFor(x => x.PatientName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.UnitsNeeded)
                .GreaterThan(0);

            RuleFor(x => x.ContactPhone)
                .MaximumLength(11);

            RuleFor(x => x.MedicalDocumentUrl)
                .MaximumLength(500);

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.ExpiresAt.HasValue);
        }
    }
}
