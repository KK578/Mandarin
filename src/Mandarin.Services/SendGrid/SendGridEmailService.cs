using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.SendGrid
{
    /// <inheritdoc />
    internal sealed class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> logger;
        private readonly ISendGridClient sendGridClient;
        private readonly IOptions<SendGridConfiguration> sendGridConfigurationOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridEmailService"/> class.
        /// </summary>
        /// <param name="sendGridClient">The SendGrid API Client.</param>
        /// <param name="sendGridConfigurationOption">The application configuration for Email customisations.</param>
        /// <param name="logger">The application logger.</param>
        public SendGridEmailService(ISendGridClient sendGridClient,
                                    IOptions<SendGridConfiguration> sendGridConfigurationOption,
                                    ILogger<SendGridEmailService> logger)
        {
            this.logger = logger;
            this.sendGridClient = sendGridClient;
            this.sendGridConfigurationOption = sendGridConfigurationOption;
        }

        /// <inheritdoc/>
        public Task<EmailResponse> SendRecordOfSalesEmailAsync(RecordOfSales recordOfSales)
        {
            return this.SendEmail(this.BuildRecordOfSalesEmail(recordOfSales));
        }

        private SendGridMessage BuildRecordOfSalesEmail(RecordOfSales recordOfSales)
        {
            var email = new SendGridMessage();
            var configuration = this.sendGridConfigurationOption.Value;
            email.From = new EmailAddress(configuration.ServiceEmail);
            email.AddTo(new EmailAddress(recordOfSales.EmailAddress));
            email.TemplateId = configuration.RecordOfSalesTemplateId;
            email.SetTemplateData(recordOfSales);

            if (!string.Equals(recordOfSales.EmailAddress, configuration.RealContactEmail, StringComparison.OrdinalIgnoreCase))
            {
                email.ReplyTo = new EmailAddress(configuration.RealContactEmail);
                email.AddBcc(configuration.RealContactEmail);
            }

            return email;
        }

        private async Task<EmailResponse> SendEmail(SendGridMessage email)
        {
            var response = await this.sendGridClient.SendEmailAsync(email);
            var bodyContent = await GetBodyContent(response.Body);
            this.logger.LogInformation("Response from SendGrid: Status={Status}, Message={Message}", response.StatusCode, bodyContent);
            return new EmailResponse((int)response.StatusCode);

            static async Task<string> GetBodyContent(HttpContent content)
            {
                if (content == null)
                {
                    return null;
                }

                try
                {
                    return await content.ReadAsStringAsync();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
