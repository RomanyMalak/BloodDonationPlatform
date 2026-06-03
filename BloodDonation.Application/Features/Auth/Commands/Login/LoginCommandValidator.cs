using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace BloodDonation.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator:AbstractValidator<LoginCommand>
    {
        //strong email validation
        public LoginCommandValidator()
        {  //strong password validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
          
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            //email and password combination validation
             RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    // Here you would typically check the database to see if the email and password combination is valid.
                    // For demonstration purposes, we'll just return true.
                    return await Task.FromResult(true);
                })
                .WithMessage("Invalid email or password");
        }
    }
}
