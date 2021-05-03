using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Services;
using UltraPro.Entities;
using UltraPro.Services.Interfaces;
using Wangkanai.Detection.Services;

namespace UltraPro.API.Common
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IDetectionService _detectionService;
        private readonly IRequestResponseLogService _requestResponseLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger,
            IDetectionService detectionService,
            IRequestResponseLogService requestResponseLogService,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _next = next;
            _logger = logger;
            _detectionService = detectionService;
            _requestResponseLogService = requestResponseLogService;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task Invoke(HttpContext context)
        {
            //Create a log
            var log = new RequestResponseLog
            {
                Id = Guid.NewGuid(),
                UserId = _currentUserService.UserId,
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.ToString(),
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                Method = context.Request.Method,
                ContentType = context.Request.ContentType
            };

            await this.LogRequest(context, log);
            await _next(context);
            await this.LogResponse(context, log);
        }

        private async Task LogRequest(HttpContext context, RequestResponseLog log)
        {
            log.Payload = await this.GetRequestBodyAsync(context.Request);
            log.RequestedOn = DateTime.Now;
        }

        private async Task LogResponse(HttpContext context, RequestResponseLog log)
        {
            //Copy a pointer to the original response body stream
            using (var originalResponseBody = context.Response.Body)
            {
                try
                {
                    //Create a new memory stream...
                    using (var responseBody = new MemoryStream())
                    {
                        //...and use that for the temporary response body
                        context.Response.Body = responseBody;

                        responseBody.Position = 0;
                        var reader = new StreamReader(responseBody);
                        var response = await reader.ReadToEndAsync();
                        responseBody.Position = 0;

                        log.Response = response;
                        log.ResponseCode = context.Response.StatusCode;
                        log.IsSuccessStatusCode = (context.Response.StatusCode == (int)HttpStatusCode.OK || 
                                                    context.Response.StatusCode == (int)HttpStatusCode.Created);
                        log.RespondedOn = DateTime.Now;

                        //TODO: Save log to database
                        await this.LogClientInformaionAsync(context, log);
                        var result = await _requestResponseLogService.AddAsync(log);

                        //Copy the contents of the new memory stream (which contains the response) to the original stream,...
                        //...which is then returned to the client.
                        await responseBody.CopyToAsync(originalResponseBody);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Request Response Logging Exception Message: {ex.Message}");
                }
                finally
                {
                    //Assign the response body to the actual context
                    context.Response.Body = originalResponseBody;
                }
            }
        }

        private async Task LogClientInformaionAsync(HttpContext context, RequestResponseLog log)
        {
            var userAgent = context.Request.Headers[HeaderNames.UserAgent].ToString();
            var acceptLanguage = context.Request.Headers[HeaderNames.AcceptLanguage].ToString();
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();

            var userAgentInfo = _detectionService.UserAgent;
            var device = _detectionService.Device;
            var platform = _detectionService.Platform;
            var engine = _detectionService.Engine;
            var browser = _detectionService.Browser;
            var crawler = _detectionService.Crawler;
        }

        private async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            string body = string.Empty;

            // body serialize for FormData
            if (request.HasFormContentType && request.Form.Any())
            {
                var dictionary = request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                body = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            }
            else
            {
                request.Body.Position = 0;
                var reader = new StreamReader(request.Body);
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return body;
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
