using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltraPro.CQRS.Products.Queries.GetProducts;
using UltraPro.CQRS.Products.Commands.DeleteProducts;
using UltraPro.CQRS.Products.Commands.UpdateProducts;
using UltraPro.CQRS.Products.Commands.CreateProducts;
using UltraPro.CQRS.Products.Queries.GetProductsById;
using UltraPro.API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;

namespace UltraPro.API.Controllers.Products
{
    //[Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/products")]
    public class ProductsController : BaseController
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetProductQuery query)
        {
            var result = await _mediator.Send(query);
            return OkResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery() { Id = id });
            return OkResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return OkResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                ModelState.AddModelError("Id", "Id is not valid.");
                return ValidationResult(ModelState);
            }

            var result = await _mediator.Send(command);
            return OkResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id });
            return OkResult(result);
        }
    }
}
