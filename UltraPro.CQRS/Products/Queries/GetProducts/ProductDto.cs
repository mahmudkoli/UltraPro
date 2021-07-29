using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Mappings;
using UltraPro.Entities;

namespace UltraPro.CQRS.Products.Queries.GetProducts
{
    public class ProductsDto : IMapFrom<Product>
    {
        public ProductsDto()
        {

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
