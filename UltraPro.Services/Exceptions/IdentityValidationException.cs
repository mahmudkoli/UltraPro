using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Services.Exceptions
{
    public class IdentityValidationException : Exception
    {
        public IDictionary<string, string> Failures { get; }

        public IdentityValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string>();
        }

        public IdentityValidationException(string message)
            : base(message)
        {
            Failures = new Dictionary<string, string>();
        }

        public IdentityValidationException(IEnumerable<IdentityError> failures)
            : this(failures.Any() ? failures.FirstOrDefault().Description : "One or more validation failures have occurred.")
        {
            foreach (var failure in failures)
            {
                var propertyName = failure.Code;
                var propertyFailure = failure.Description;

                Failures.Add(propertyName, propertyFailure);
            }
        }
    }
}
