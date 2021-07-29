using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Common.Exceptions
{
    public class IdentityValidationException : Exception
    {
        public IDictionary<string, string[]> Failures { get; }

        public IdentityValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public IdentityValidationException(string message)
            : base(message)
        {
            Failures = new Dictionary<string, string[]>();
        }

        public IdentityValidationException(IEnumerable<IdentityError> failures)
            : this(failures.Any() ? failures.FirstOrDefault().Description : "One or more validation failures have occurred.")
        {
            var groupFailures = failures.GroupBy(x => x.Code);

            foreach (var failure in groupFailures)
            {
                var propertyName = failure.Key;
                var propertyFailure = failure.Select(x => x.Description).ToArray();

                Failures.Add(propertyName, propertyFailure);
            }
        }
    }
}
