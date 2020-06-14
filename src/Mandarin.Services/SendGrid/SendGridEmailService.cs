using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Bashi.Core.Enums;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
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
        public async Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            Validator.ValidateObject(model, new ValidationContext(model), true);

            var email = new SendGridMessage();
            var configuration = this.sendGridConfigurationOption.Value;
            email.From = new EmailAddress(configuration.ServiceEmail);
            email.ReplyTo = new EmailAddress(model.Email);
            email.AddTo(new EmailAddress(configuration.RealContactEmail));
            email.Subject = $"{model.Name} - {(model.Reason == ContactReasonType.Other ? $"Other ({model.AdditionalReason})" : model.Reason.GetDescription())}";

            foreach (var attachment in model.Attachments)
            {
                await email.AddAttachmentAsync(attachment.Name, attachment.Data, attachment.Type);
            }

            var sb = new StringBuilder()
                     .AppendLine($"Reason: {(model.Reason == ContactReasonType.Other ? model.AdditionalReason : model.Reason.GetDescription())}")
                     .AppendLine()
                     .AppendLine("Comment:")
                     .AppendLine(model.Comment);
            email.PlainTextContent = sb.ToString();

            return email;
        }

        /// <inheritdoc/>
        public SendGridMessage BuildRecordOfSalesEmail(ArtistSales artistSales)
        {
            var email = new SendGridMessage();
            var configuration = this.sendGridConfigurationOption.Value;
            email.From = new EmailAddress(configuration.ServiceEmail);
            email.ReplyTo = new EmailAddress(configuration.RealContactEmail);
            email.AddTo(new EmailAddress(artistSales.EmailAddress));
            email.AddBcc(configuration.RealContactEmail);
            email.TemplateId = configuration.RecordOfSalesTemplateId;
            email.SetTemplateData(artistSales);

            return email;
        }

        /// <inheritdoc/>
        public async Task<EmailResponse> SendEmailAsync(SendGridMessage email)
        {
            var response = await this.sendGridClient.SendEmailAsync(email);
            var bodyContent = await SendGridEmailService.GetBodyContent(response);
            this.logger.LogInformation("SendGrid SendEmail: Status={Status}, Message={Message}", response.StatusCode, bodyContent);
            return new EmailResponse((int)response.StatusCode);
        }

        private static async Task<string> GetBodyContent(Response response)
        {
            if (response.Body == null)
            {
                return null;
            }

            var bodyContent = await response.Body.ReadAsStringAsync();
            return bodyContent;
        }
    }
}
