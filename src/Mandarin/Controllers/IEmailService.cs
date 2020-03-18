using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mandarin.Controllers
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync();
    }

    internal sealed class EmailService : IEmailService
    {
        public Task<EmailResponse> SendEmailAsync()
        {
            return Task.FromResult(new EmailResponse(StatusCodes.Status202Accepted));
        }
    }
}
