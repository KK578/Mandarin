using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Models.Stockists;

namespace Mandarin.Database.Stockists
{
    /// <inheritdoc />
    internal sealed class StockistRepository : IStockistRepository
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public StockistRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCode(string stockistCode)
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT s.*, sd.*
                                 FROM inventory.stockist s
                                 INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
                                 WHERE s.stockist_code = @StockistCode
                                 ORDER BY stockist_code
                                 LIMIT 1";
            var stockistRecords = await connection.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(sql,
                (s, sd) => s with { Details = sd },
                new { StockistCode = stockistCode },
                splitOn: "stockist_id,stockist_id");

            var stockist = this.mapper.Map<Stockist>(stockistRecords.FirstOrDefault());
            return stockist;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetAllStockists()
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT s.*, sd.*
                                 FROM inventory.stockist s
                                 INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
                                 ORDER BY stockist_code";
            var stockistRecords = await connection.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(sql,
                (s, sd) => s with { Details = sd },
                splitOn: "stockist_id,stockist_id");

            var stockists = this.mapper.Map<List<Stockist>>(stockistRecords).AsReadOnly();
            return stockists;
        }
    }
}
