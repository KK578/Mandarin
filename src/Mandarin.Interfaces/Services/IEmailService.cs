using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services
{
    public interface IEmailService
    {
        Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model);
        SendGridMessage BuildRecordOfSalesEmail(ArtistRecordOfSalesModel recordOfSalesModel);
        Task<EmailResponse> SendEmailAsync(SendGridMessage email);
    }
}
