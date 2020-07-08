using System;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using Mandarin.Services.Square;
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
            try
            {
                this.WriteLog(email);
            }
            catch
            {
                // Ignore errors while trying to log.
            }

            try
            {
                return await this.emailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception whilst attempting to send email on behalf of {From}.", email.From.Email);
                throw;
            }
        }

        private void WriteLog(SendGridMessage email)
        {
            if (string.IsNullOrEmpty(email.Subject))
            {
                foreach (var personalization in email.Personalizations.NullToEmpty())
                {
                    this.logger.LogInformation("Sending Record of Sales Email: From={From}, To={To}, Data={Data}",
                                               email.From.Email,
                                               string.Join(" & ", personalization.Tos.NullToEmpty().Select(x => x.Email)),
                                               personalization.TemplateData);
                }
            }
            else
            {
                this.logger.LogInformation("Sending Contact Form Email: From={From}, Subject={Subject}, Content={Content}, Attachments={Attachments}",
                                           email.From.Email,
                                           email.Subject,
                                           email.PlainTextContent,
                                           email.Attachments?.Count ?? 0);
            }
        }
    }
}
