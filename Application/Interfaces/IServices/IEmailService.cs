using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
