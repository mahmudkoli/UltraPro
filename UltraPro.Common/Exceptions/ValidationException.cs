using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraPro.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Failures { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(string message)
            : base(message)
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(List<(string PropertyName, string[] PropertyFailures)> failures)
            : this(failures?.FirstOrDefault().PropertyFailures?.FirstOrDefault() ?? "One or more validation failures have occurred.")
        {
            foreach (var failure in failures)
            {
                var propertyName = failure.PropertyName;
                var propertyFailures = failure.PropertyFailures;

                Failures.Add(propertyName, propertyFailures);
            }
        }

        public ValidationException(List<ValidationError> failures)
            : this(failures?.FirstOrDefault()?.PropertyFailures?.FirstOrDefault() ?? "One or more validation failures have occurred.")
        {
            foreach (var failure in failures)
            {
                var propertyName = failure.PropertyName;
                var propertyFailures = failure.PropertyFailures.ToArray();

                Failures.Add(propertyName, propertyFailures);
            }
        }
    }
}
