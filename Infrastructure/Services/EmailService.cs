using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Services
{
        //Application decides WHEN to send email
        //Infrastructure decides HOW to send email

    public class EmailService : IEmailService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            Console.WriteLine($"Email sent to {to}");
            await Task.CompletedTask;
        }
    }
}






