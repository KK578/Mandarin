﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Commissions;
using Mandarin.Commissions;
using Microsoft.AspNetCore.Authorization;
using NodaTime;
using static Mandarin.Api.Commissions.Commissions;
using RecordOfSales = Mandarin.Api.Commissions.RecordOfSales;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class CommissionsGrpcService : CommissionsBase
    {
        private readonly IRecordOfSalesRepository recordOfSalesRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionsGrpcService"/> class.
        /// </summary>
        /// <param name="recordOfSalesRepository">The application service for interacting with records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public CommissionsGrpcService(IRecordOfSalesRepository recordOfSalesRepository, IMapper mapper)
        {
            this.recordOfSalesRepository = recordOfSalesRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetRecordOfSalesForPeriodResponse> GetRecordOfSalesForPeriod(GetRecordOfSalesForPeriodRequest request, ServerCallContext context)
        {
            var startDate = this.mapper.Map<LocalDate>(request.StartDate);
            var endDate = this.mapper.Map<LocalDate>(request.EndDate);
            var interval = new DateInterval(startDate, endDate);
            var recordOfSales = await this.recordOfSalesRepository.GetRecordOfSalesAsync(interval);

            return new GetRecordOfSalesForPeriodResponse
            {
                Sales = { this.mapper.Map<List<RecordOfSales>>(recordOfSales) },
            };
        }
    }
}
