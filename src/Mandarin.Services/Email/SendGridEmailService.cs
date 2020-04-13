﻿using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Bashi.Core.Enums;
using Mandarin.Models.Contact;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Email
{
    internal sealed class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> logger;
        private readonly ISendGridClient sendGridClient;

        public SendGridEmailService(ISendGridClient sendGridClient, ILogger<SendGridEmailService> logger)
        {
            this.logger = logger;
            this.sendGridClient = sendGridClient;
        }

        public async Task<SendGridMessage> BuildEmailAsync(ContactDetailsModel model)
        {
            Validator.ValidateObject(model, new ValidationContext(model), true);

            var email = new SendGridMessage();
            email.From = new EmailAddress(model.Email);
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

        public async Task<EmailResponse> SendEmailAsync(SendGridMessage email)
        {
            var response = await this.sendGridClient.SendEmailAsync(email);
            var bodyContent = await response.Body.ReadAsStringAsync();
            this.logger.LogInformation("SendGrid SendEmail: Status={Status}, Message={Message}", response.StatusCode, bodyContent);
            return new EmailResponse((int)response.StatusCode);
        }
    }
}