using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.CQRS.Products.Queries.GetProducts
{
    public class ProductsVm
    {
        public IList<ProductsDto> Lists { get; set; }
    }
}
