using System.Text;
using System.Threading.Tasks;
using Mandarin.Extensions;
using Mandarin.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Email
{
    internal sealed class EmailService : IEmailService
    {
        private readonly ISendGridClient sendGridClient;

        public EmailService()
        {
            this.sendGridClient = new SendGridClient("");
        }

        public async Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            var email = new SendGridMessage();
            email.From = new EmailAddress(model.Email);
            email.Subject = model.Name + " - " + model.Reason.GetDescription();

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

        public async Task<EmailResponse> SendEmailAsync(SendGridMessage sendGridMessage)
        {
            var response = await this.sendGridClient.SendEmailAsync(sendGridMessage);
            return new EmailResponse((int)response.StatusCode);
        }
    }
}
