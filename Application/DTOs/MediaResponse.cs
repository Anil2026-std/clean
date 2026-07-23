using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class MediaResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
