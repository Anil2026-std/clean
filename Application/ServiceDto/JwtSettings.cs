using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceDto
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
