using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mandarin.Services.Email
{
    internal sealed class EmailService : IEmailService
    {
        public Task<EmailResponse> SendEmailAsync()
        {
            return Task.FromResult(new EmailResponse(StatusCodes.Status202Accepted));
        }
    }
}
