using System;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Emails;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.Services.Objects;
using static Mandarin.Api.Emails.Emails;

namespace Mandarin.Client.Services.Emails
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcEmailService : IEmailService
    {
        private readonly EmailsClient emailsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcEmailService"/> class.
        /// </summary>
        /// <param name="emailsClient">The gRPC client to Mandarin API for Emails.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcEmailService(EmailsClient emailsClient, IMapper mapper)
        {
            this.emailsClient = emailsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<EmailResponse> SendRecordOfSalesEmailAsync(RecordOfSales recordOfSales)
        {
            var request = new SendRecordOfSalesEmailRequest
            {
                RecordOfSales = this.mapper.Map<Api.Commissions.RecordOfSales>(recordOfSales),
            };
            var response = await this.emailsClient.SendRecordOfSalesEmailAsync(request);
            return new EmailResponse { IsSuccess = response.IsSuccess, Message = response.Message };
        }
    }
}
