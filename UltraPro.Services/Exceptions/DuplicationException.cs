using System;
using System.Collections.Generic;
using System.Text;

namespace UltraPro.Services.Exceptions
{
    public class DuplicationException : Exception
    {
        public DuplicationException(string name)
            : base($"{name} already exists.")
        {
        }
    }
}
