using FluentValidation;

namespace BloodDonation.Application.Features.Auth.Commands.CreateAdmin;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name max 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(150).WithMessage("Email max 150 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password min 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain special character.");

        RuleFor(x => x.Phone)
            .Matches(@"^01[0125][0-9]{8}$")
            .WithMessage("Invalid Egyptian phone number.")
            .When(x => x.Phone is not null);
    }
}