using System;
namespace RegAuthApiDemo.Service
{
	public interface IEmailService
	{
		Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message);
	}
}

