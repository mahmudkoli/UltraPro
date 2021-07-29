using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;

namespace UltraPro.API.Models.Validators
{
    public class SaveSingleValueValidator : AbstractValidator<SaveSingleValueDetailModel>
    {
        public SaveSingleValueValidator()
        {
            RuleFor(v => v.Name)
                //.Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("Name is required.")
                .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
                .Length(2, 25).WithMessage("{PropertyValue} must be more than 2 letter.");
            RuleFor(v => v.Code)
                //.Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("Code is required.")
                .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
                .Length(2, 25).WithMessage("{PropertyValue} must be more than 2 letter.");

            RuleFor(x => x.TypeId)
                .Must(x => x > 0)
                .WithMessage("Must be select type.");
        }
    }
}
