﻿using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can send emails via SendGrid.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Generates an email from a public user attempting to contact The Little Mandarin.
        /// </summary>
        /// <param name="model">Model containing the user's contact form details.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a prepared email to be sent via <see cref="SendEmailAsync"/>.</returns>
        Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model);

        /// <summary>
        /// Generates an email from The Little Mandarin to an Artist, with details of their commission for a time period.
        /// </summary>
        /// <param name="artistSales">Model containing the artist's commission details.</param>
        /// <returns>A prepared email to be sent via <see cref="SendEmailAsync"/>.</returns>
        SendGridMessage BuildRecordOfSalesEmail(ArtistSales artistSales);

        /// <summary>
        /// Sends the provided email to the SendGrid API.
        /// </summary>
        /// <param name="email">Prepared email to be sent.</param>
        /// <returns>SendGrid's API response.</returns>
        Task<EmailResponse> SendEmailAsync(SendGridMessage email);
    }
}
