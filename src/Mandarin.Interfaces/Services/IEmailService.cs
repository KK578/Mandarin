using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services.Objects;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can send emails via SendGrid.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the provided email to the SendGrid API.
        /// </summary>
        /// <param name="recordOfSales">The artist commission breakdown.</param>
        /// <returns>SendGrid's API response.</returns>
        Task<EmailResponse> SendRecordOfSalesEmailAsync(RecordOfSales recordOfSales);
    }
}
