using System;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Decorated implementation of <see cref="IEmailService"/> that logs emails as they are sent.
    /// </summary>
    internal sealed class LoggingEmailServiceDecorator : IEmailService
    {
        private readonly ILogger<IEmailService> logger;
        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEmailServiceDecorator"/> class.
        /// </summary>
        /// <param name="emailService">The email service to be decorated.</param>
        /// <param name="logger">The application to log to.</param>
        public LoggingEmailServiceDecorator(IEmailService emailService, ILogger<IEmailService> logger)
        {
            this.logger = logger;
            this.emailService = emailService;
        }

        /// <inheritdoc/>
        public Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            return this.emailService.BuildEmailAsync(model);
        }

        /// <inheritdoc/>
        public SendGridMessage BuildRecordOfSalesEmail(ArtistSales artistSales)
        {
            return this.emailService.BuildRecordOfSalesEmail(artistSales);
        }

        /// <inheritdoc/>
        public async Task<EmailResponse> SendEmailAsync(SendGridMessage email)
        {
            this.logger.LogInformation("Sending Email: From={From}, Subject={Subject}, Content={Content}, Attachments={Attachments}",
                                       email.From.Email,
                                       email.Subject,
                                       email.PlainTextContent,
                                       email.Attachments?.Count ?? 0);

            try
            {
                return await this.emailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex,
                                     "Exception whilst attempting to send email on behalf of {From}.",
                                     email.From.Email);
                throw;
            }
        }
    }
}
