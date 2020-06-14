namespace Mandarin.Services.SendGrid
{
    /// <summary>
    /// Application configuration for the SendGrid API Client.
    /// </summary>
    internal class SendGridConfiguration
    {
        /// <summary>
        /// Gets or sets the email address that should be used as the 'Sent by' in all automated emails.
        /// </summary>
        public string ServiceEmail { get; set; }

        /// <summary>
        /// Gets or sets the email address that should be used as the 'Reply to' in all automated emails.
        /// </summary>
        public string RealContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the SendGrid template ID for the automated record of sales emails.
        /// <br/>
        /// See <a href="https://sendgrid.com/docs/ui/sending-email/how-to-send-an-email-with-dynamic-transactional-templates/">SendGrid Documentation</a>
        /// on Templated emails.
        /// </summary>
        public string RecordOfSalesTemplateId { get; set; }
    }
}
