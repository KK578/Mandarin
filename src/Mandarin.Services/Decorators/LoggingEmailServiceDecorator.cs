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
        public async Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            var email = await this.emailService.BuildEmailAsync(model);
            this.logger.LogInformation("Sending Contact Form Email: {@Email}", email);
            return email;
        }

        /// <inheritdoc/>
        public SendGridMessage BuildRecordOfSalesEmail(ArtistSales artistSales)
        {
            var email = this.emailService.BuildRecordOfSalesEmail(artistSales);
            this.logger.LogInformation("Sending Record of Sales Email: {@Email}", email);
            return email;
        }

        /// <inheritdoc/>
        public async Task<EmailResponse> SendEmailAsync(SendGridMessage email)
        {
            try
            {
                var response = await this.emailService.SendEmailAsync(email);

                if (response.IsSuccess)
                {
                    this.logger.LogInformation("Sent email successfully: {@Email}", email);
                }
                else
                {
                    this.logger.LogWarning("Email sent with errors: {@Response} {@Email}", response, email);
                }

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception whilst attempting to send email: {@Email}.", email);
                throw;
            }
        }
    }
}
