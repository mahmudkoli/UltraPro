using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraPro.Common.Exceptions;
using UltraPro.Entities;
using UltraPro.Repositories.Context;

namespace UltraPro.CQRS.Products.Commands.UpdateProducts
{
    public class UpdateProductCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProductCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
