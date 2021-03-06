﻿using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Emails;
using Mandarin.Commissions;
using Mandarin.Emails;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Emails.Emails;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class EmailGrpcService : EmailsBase
    {
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailGrpcService"/> class.
        /// </summary>
        /// <param name="emailService">The application service for sending emails.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public EmailGrpcService(IEmailService emailService, IMapper mapper)
        {
            this.emailService = emailService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<SendRecordOfSalesEmailResponse> SendRecordOfSalesEmail(SendRecordOfSalesEmailRequest request, ServerCallContext context)
        {
            var recordOfSales = this.mapper.Map<RecordOfSales>(request.RecordOfSales);
            var response = await this.emailService.SendRecordOfSalesEmailAsync(recordOfSales);

            return new SendRecordOfSalesEmailResponse
            {
                IsSuccess = response.IsSuccess,
                Message = response.Message,
            };
        }
    }
}
