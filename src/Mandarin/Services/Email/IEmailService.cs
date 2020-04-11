using System.Threading.Tasks;
using Mandarin.Models;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Email
{
    public interface IEmailService
    {
        Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model);
        Task<EmailResponse> SendEmailAsync(SendGridMessage email);
    }
}
