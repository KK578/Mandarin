using System;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Decorators
{
    internal sealed class LoggingEmailServiceDecorator : IEmailService
    {
        private readonly ILogger<IEmailService> logger;
        private readonly IEmailService emailService;

        public LoggingEmailServiceDecorator(IEmailService emailService, ILogger<IEmailService> logger)
        {
            this.logger = logger;
            this.emailService = emailService;
        }

        public Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            return this.emailService.BuildEmailAsync(model);
        }

        public SendGridMessage BuildRecordOfSalesEmail(ArtistSales artistSales)
        {
            return this.emailService.BuildRecordOfSalesEmail(artistSales);
        }

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
