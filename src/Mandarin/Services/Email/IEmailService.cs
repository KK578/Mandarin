using System.Threading.Tasks;

namespace Mandarin.Services.Email
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync();
    }
}
