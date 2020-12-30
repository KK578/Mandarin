using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services.Objects;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can send emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the <see cref="RecordOfSales"/> to the attached email address.
        /// </summary>
        /// <param name="recordOfSales">The artist commission breakdown.</param>
        /// <returns>SendGrid's API response.</returns>
        Task<EmailResponse> SendRecordOfSalesEmailAsync(RecordOfSales recordOfSales);
    }
}
