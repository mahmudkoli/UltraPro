using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;

namespace UltraPro.API.Models.Validators
{
    public class SaveUserRoleValidator : AbstractValidator<SaveUserRoleModel>
    {
        public SaveUserRoleValidator()
        {
            RuleFor(v => v.Name)
                //.Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("Name is required.")
                .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
                .Length(2, 25).WithMessage("{PropertyValue} must be more than 2 letter.");

            //RuleForEach(x => x.Permissions).NotEmpty().WithMessage("Permission {CollectionIndex} is required.");

            //RuleForEach(x => x.Permissions).ChildRules(permissions =>
            //{
            //    permissions.RuleFor(x => x).NotEmpty().WithMessage("Permission is required.");
            //});
            
            RuleFor(x => x.Permissions)
                .Must(x => x.Count() > 0)
                .WithMessage("At least one permission should be added.");


            //RuleForEach(v => v.Permissions).SetValidator(new StringValidator());
        }
    }
}
