using System;
using System.Net;
using System.Net.Mail;

namespace RegAuthApiDemo.Service
{
	public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
            _configuration = configuration;
		}

        public async Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message)
        {
            var mail = new MailMessage(fromAddress, toAddress, subject, message);

            using (var client = new SmtpClient(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]))
            {
                Credentials = new NetworkCredential(_configuration["SMTP:Username"], _configuration["SMPT:Password"])
            })
            {
                await client.SendMailAsync(mail);
            }


              

           
        }
    }
}

