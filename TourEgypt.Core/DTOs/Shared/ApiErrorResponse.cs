using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.DTOs.Shared
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}
