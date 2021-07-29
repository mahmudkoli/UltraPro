using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltraPro.API.Core;
using UltraPro.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UltraPro.Common.Exceptions;
using System.Net;

namespace UltraPro.API.Controllers.Common
{
    //[ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult OkResult(object data)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Status = ApiResponseStatus.Success.ToString(),
                Message = "The operation has been successful.",
                Data = data
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult OkResult(object data, string message)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Status = ApiResponseStatus.Success.ToString(),
                Message = message,
                Data = data
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult ValidationResult(ModelStateDictionary modelState)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Status = ApiResponseStatus.ValidationError.ToString(),
                Message = "One or more validation failures have occurred.",
                Errors = modelState.GetErrors()
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult BadRequestResult(Exception ex)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Status = ApiResponseStatus.Error.ToString(),
                Message = ex.Message,
                Data = new object(),
                Errors = new List<ValidationError> { new ValidationError { PropertyName = "", PropertyFailures = new string[] { ex.Message } } }
            };
            return ObjectResult(apiResult);
        }

        protected IActionResult ExceptionResult(Exception ex)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Status = ApiResponseStatus.Error.ToString(),
                Message = ex.Message,
                Data = new object(),
                Errors = new List<ValidationError> { new ValidationError { PropertyName = "", PropertyFailures = new string[] { ex.Message } } }
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
