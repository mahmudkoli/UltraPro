using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltraPro.API.Core;
using UltraPro.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UltraPro.API.Controllers.Common
{
    //[ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult OkResult(object data)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 200,
                Status = "Success",
                Message = "Successful",
                Data = data
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult OkResult(object data, string message)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 200,
                Status = "Success",
                Message = message,
                Data = data
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult ValidationResult(ModelStateDictionary modelState)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 400,
                Status = "ValidationError",
                Message = "Validation Fail",
                Errors = modelState.GetErrors()
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult BadRequestResult(Exception ex)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 400,
                Status = "Error",
                Message = ex.Message,
                Data = new object(),
                Errors = new List<ValidationError> { new ValidationError { PropertyName = "", Errors = new string[] { ex.Message } } }
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult ExceptionResult(Exception ex)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 500,
                Status = "Error",
                Message = ex.Message,
                Data = new object(),
                Errors = new List<ValidationError> { new ValidationError { PropertyName = "", Errors = new string[] { ex.Message } } }
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult ObjectResult(ApiResponse model)
        {
            var result = new ObjectResult(model)
            {
                StatusCode = model.StatusCode
            };
            return result;
        }
    }
}
