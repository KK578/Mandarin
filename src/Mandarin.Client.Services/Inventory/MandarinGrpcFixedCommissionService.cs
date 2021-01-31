using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using static Mandarin.Api.Inventory.FixedCommissions;
using FixedCommissionAmount = Mandarin.Inventory.FixedCommissionAmount;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcFixedCommissionService : IFixedCommissionService
    {
        private readonly FixedCommissionsClient fixedCommissionsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcFixedCommissionService"/> class.
        /// </summary>
        /// <param name="fixedCommissionsClient">The gRPC client to Mandarin API for FixedCommissions.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcFixedCommissionService(FixedCommissionsClient fixedCommissionsClient, IMapper mapper)
        {
            this.fixedCommissionsClient = fixedCommissionsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<FixedCommissionAmount>> GetFixedCommissionAsync()
        {
            var request = new GetAllFixedCommissionsRequest();
            var response = await this.fixedCommissionsClient.GetAllFixedCommissionsAsync(request);
            return this.mapper.Map<List<FixedCommissionAmount>>(response.FixedCommissions).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task<FixedCommissionAmount> GetFixedCommissionAsync(string productCode)
        {
            var request = new GetFixedCommissionRequest { ProductCode = productCode };
            var response = await this.fixedCommissionsClient.GetFixedCommissionAsync(request);
            return this.mapper.Map<FixedCommissionAmount>(response.FixedCommission);
        }

        /// <inheritdoc/>
        public async Task SaveFixedCommissionAsync(FixedCommissionAmount commission)
        {
            var request = new SaveFixedCommissionRequest { FixedCommission = this.mapper.Map<Api.Inventory.FixedCommissionAmount>(commission) };
            await this.fixedCommissionsClient.SaveFixedCommissionAsync(request);
        }

        /// <inheritdoc/>
        public async Task DeleteFixedCommissionAsync(string productCode)
        {
            var request = new DeleteFixedCommissionRequest { ProductCode = productCode };
            await this.fixedCommissionsClient.DeleteFixedCommissionAsync(request);
        }
    }
}
