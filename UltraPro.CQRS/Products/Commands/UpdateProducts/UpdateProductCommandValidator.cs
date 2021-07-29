using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;

namespace UltraPro.CQRS.Products.Commands.UpdateProducts
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProductCommandValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(v => v.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(20).WithMessage("Code must not exceed 20 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified code already exists.");
        }

        public async Task<bool> BeUniqueTitle(UpdateProductCommand model, string code, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Where(l => l.Id != model.Id)
                .AllAsync(l => l.Code != code);
        }
    }
}
