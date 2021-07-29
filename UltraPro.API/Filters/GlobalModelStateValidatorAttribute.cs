using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UltraPro.API.Core;
using UltraPro.API.Extensions;
using UltraPro.Common.Exceptions;

namespace UltraPro.API.Filters
{
    public class GlobalModelStateValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //Controller controller = context.Controller as Controller;
                //object model = context.ActionArguments.Any()
                //   ? context.ActionArguments.First().Value
                //   : null;

                //context.Result = (IActionResult)controller?.View(model)
                //   ?? new BadRequestResult();

                //context.Result = new ContentResult()
                //{
                //    Content = "ModelState is not valid.",
                //    StatusCode = 400
                //};

                //context.Result = new BadRequestObjectResult(context.ModelState);

                throw new ValidationException(context.ModelState.GetErrors());
                //throw new ValidationException();

                //object model = context.ActionArguments.ContainsKey("model") ? context.ActionArguments["model"] : new object();
                //var apiResult = new ApiResponse
                //{
                //    Data = model,
                //    StatusCode = 400,
                //    Status = "ValidationError",
                //    Message = "Validation Failed.",
                //    Errors = context.ModelState.GetErrors()
                //};
                //context.Result = new BadRequestObjectResult(apiResult);
            }

            base.OnActionExecuting(context);
        }
    }
}
