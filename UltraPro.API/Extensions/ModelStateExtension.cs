using UltraPro.API.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Services.Exceptions;

namespace UltraPro.API.Extensions
{
    public static class ModelStateExtension
    {
        public static List<ValidationError> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = (from m in modelState
                            where m.Value.Errors.Count() > 0
                            select new ValidationError
                            {
                                PropertyName = m.Key,
                                PropertyFailures = (from msg in m.Value.Errors
                                                    select msg.ErrorMessage).ToArray()
                            })
                            .ToList();
            return errors;
        }
    }
}
