using System.Threading.Tasks;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services
{
    public interface IEmailService
    {
        Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model);
        Task<EmailResponse> SendEmailAsync(SendGridMessage email);
    }
}
