using System;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Decorated implementation of <see cref="IEmailService"/> that logs emails as they are sent.
    /// </summary>
    internal sealed class LoggingEmailServiceDecorator : IEmailService
    {
        private readonly ILogger<LoggingEmailServiceDecorator> logger;
        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEmailServiceDecorator"/> class.
        /// </summary>
        /// <param name="emailService">The email service to be decorated.</param>
        /// <param name="logger">The application to log to.</param>
        public LoggingEmailServiceDecorator(IEmailService emailService, ILogger<LoggingEmailServiceDecorator> logger)
        {
            this.logger = logger;
            this.emailService = emailService;
        }

        /// <inheritdoc/>
        public async Task<EmailResponse> SendRecordOfSalesEmailAsync(RecordOfSales recordOfSales)
        {
            try
            {
                var response = await this.emailService.SendRecordOfSalesEmailAsync(recordOfSales);

                if (response.IsSuccess)
                {
                    this.logger.LogInformation("Sent email successfully: {@Email}", recordOfSales);
                }
                else
                {
                    this.logger.LogWarning("Email sent with errors: {@Response} {@Email}", response, recordOfSales);
                }

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception whilst attempting to send email: {@Email}.", recordOfSales);
                throw;
            }
        }
    }
}
