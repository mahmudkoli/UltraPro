using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.API.Common
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Create a log
            var log = new RequestResponseLog
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.ToString()
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
                        var temp = log;
                        var userAgent = context.Request.Headers[HeaderNames.UserAgent].ToString();
                        var acceptLanguage = context.Request.Headers[HeaderNames.AcceptLanguage].ToString();
                        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();

                        //Copy the contents of the new memory stream (which contains the response) to the original stream,...
                        //...which is then returned to the client.
                        await responseBody.CopyToAsync(originalResponseBody);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    //Assign the response body to the actual context
                    context.Response.Body = originalResponseBody;
                }
            }
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

    public class RequestResponseLog
    {
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Method { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public int ResponseCode { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime RespondedOn { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
