using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;

namespace UltraPro.CQRS.Products.Queries.GetProducts
{
    public class GetProductQuery : IRequest<ProductsVm>
    {
        public string Name { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductsVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductsVm> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            return new ProductsVm
            {
                Lists = await _context.Products
                    .Where(x => (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)))
                    .AsNoTracking()
                    .ProjectTo<ProductsDto>(_mapper.ConfigurationProvider)
                    .OrderBy(t => t.Name)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}
