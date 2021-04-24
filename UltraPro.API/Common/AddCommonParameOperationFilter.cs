using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltraPro.API.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UltraPro.API.Common
{
    public class AddCommonParameOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null && (descriptor.ControllerName.Equals("ApiAuthenticationApp")))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = nameof(AppSettings.AppHeaderSecretKey),
                    In = ParameterLocation.Header,
                    //Description = "App Header Secret Key",
                    Required = true
                });
            }
        }
    }
}
