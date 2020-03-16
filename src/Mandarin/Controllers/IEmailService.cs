using Microsoft.AspNetCore.Http;

namespace Mandarin.Controllers
{
    public interface IEmailService
    {
        EmailResponse SendEmailAsync();
    }

    internal sealed class EmailService : IEmailService
    {
        public EmailResponse SendEmailAsync()
        {
            return new EmailResponse(StatusCodes.Status202Accepted);
        }
    }
}
