using FluentValidation;
using Identity.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.FluentValidation
{
    public class RegisterUserValidator : AbstractValidator<RegisterUser>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(\+375)\d{9}$").WithMessage("Phone number is not valid.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
