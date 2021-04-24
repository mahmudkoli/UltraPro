using UltraPro.Services.Interfaces;
using System;
using UltraPro.Common.Services;

namespace UltraPro.API.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
