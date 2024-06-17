using Cedepp.Application.Contracts.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.Validators
{
    public class ChangeUserProfileDTOValidator : AbstractValidator<ChangeUserProfileDTO>
    {
        public ChangeUserProfileDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("The name is required.")
                .MaximumLength(50).WithMessage("Maximum - 50 symbols.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("The last name is required.")
                .MaximumLength(50).WithMessage("Maximum - 50 symbols.");

            RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Maximum - 20 symbols.")
            .Matches(@"^\+39\d{8,15}$").WithMessage("Phone number must start with +39 and contain 8 to 15 digits after the country code.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("The address is required.")
                .MaximumLength(100).WithMessage("Maximum - 100 symbols.");

            RuleFor(x => x.CodiceFiscale)
                .NotEmpty().WithMessage("Codice Fiscale is required.")
                .Length(16).WithMessage("Maximum - 16 symbols.");

            RuleFor(x => x.CAP)
                .NotEmpty().WithMessage("CAP is required.")
                .Length(5).WithMessage("The length should be 5");

            RuleFor(x => x.DayOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required.");

            RuleFor(x => x.Workplace)
                .MaximumLength(100).WithMessage("Workplace must not exceed 100 characters.");
        }
    }
}