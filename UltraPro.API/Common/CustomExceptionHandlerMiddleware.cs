using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UltraPro.API.Core;
using UltraPro.Services.Exceptions;

namespace UltraPro.API.Common
{
    public class CustomExceptionHandlerMiddleware
    {
        private const string JsonContentType = "application/json";
        private readonly RequestDelegate _next;
        //private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next, 
            //ILogger<CustomExceptionHandlerMiddleware> logger
            )
        {
            _next = next;
            //_logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //WriteExceptionAsync(context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;

            var apiResponse = new ApiResponse();
            apiResponse.Status = ApiResponseStatus.Error.ToString();

            switch (exception)
            {
                case IdentityValidationException validationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Errors = this.GetValidationErrors(validationException.Failures);
                    apiResponse.Status = ApiResponseStatus.ValidationError.ToString();
                    break;
                case ValidationException validationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Errors = this.GetValidationErrors(validationException.Failures);
                    apiResponse.Status = ApiResponseStatus.ValidationError.ToString();
                    break;
                case NotFoundException _:
                    httpStatusCode = HttpStatusCode.NotFound;
                    break;
                case DuplicationException _:
                    httpStatusCode = HttpStatusCode.Conflict;
                    break;
            }

            context.Response.ContentType = JsonContentType;
            context.Response.StatusCode = (int)httpStatusCode;

            apiResponse.StatusCode = (int)httpStatusCode;
            apiResponse.Message = exception.Message;

            //context.Response.Headers.Clear();

            return context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
        }

        //private void WriteExceptionAsync(HttpContext context, Exception exception)
        //{
        //    _logger.LogError(exception, exception.Message);
        //}

        private List<ValidationError> GetValidationErrors(IDictionary<string, string[]> errors)
        {
            return errors.Select(x => new ValidationError { PropertyName = x.Key, PropertyFailures = x.Value }).ToList();
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
