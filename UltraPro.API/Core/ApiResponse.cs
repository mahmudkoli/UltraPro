using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.API.Core
{
    public class ApiResponse<T> where T : class
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        public ApiResponse()
        {
            StatusCode = 200;
            Status = string.Empty;
            Message = string.Empty;
            Errors = new List<ValidationError>();
        }

        public void CreateInstance()
        {
            Data = Activator.CreateInstance<T>();
        }
    }

    public class ApiResponse
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        public ApiResponse()
        {
            StatusCode = 200;
            Status = string.Empty;
            Message = string.Empty;
            Errors = new List<ValidationError>();
            Data = new object();
        }
    }
}
