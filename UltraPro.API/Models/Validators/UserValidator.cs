using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;

namespace UltraPro.API.Models.Validators
{
    public class SaveUserValidator : AbstractValidator<SaveUserModel>
    {
        public SaveUserValidator()
        {
            RuleFor(v => v.FullName)
                .NotEmpty().WithMessage("FullName is required.");
        }
    }
}
