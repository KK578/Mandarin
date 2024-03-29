﻿using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Inventory;

namespace Mandarin.Database.Inventory
{
    /// <inheritdoc cref="Mandarin.Inventory.IProductRepository" />
    internal sealed class ProductRepository : DatabaseRepositoryBase<Product, ProductRecord>, IProductRepository
    {
        private const string GetAllProductsSql = @"
            SELECT *
            FROM inventory.product
            ORDER BY product_code";

        private const string GetProductByIdSql = @"
            SELECT *
            FROM inventory.product
            WHERE product_id = @product_id";

        private const string GetProductByCodeSql = @"
            SELECT *
            FROM inventory.product
            WHERE product_code = @product_code";

        private const string GetProductByNameSql = @"
            SELECT *
            FROM inventory.product
            WHERE product_name ILIKE '%' || @product_name || '%'";

        private const string UpsertProductSql = @"
            CALL inventory.sp_product_upsert(@product_id, @product_code, @product_name, @description, @unit_price, @last_updated)";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public ProductRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
            : base(mandarinDbContext, mapper)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            return this.GetAll(db => db.QueryAsync<ProductRecord>(ProductRepository.GetAllProductsSql));
        }

        /// <inheritdoc />
        public Task<Product> GetProductAsync(ProductId productId)
        {
            return this.Get(productId,
                            db =>
                            {
                                var parameters = new { product_id = productId.Value };
                                return db.QueryFirstOrDefaultAsync<ProductRecord>(ProductRepository.GetProductByIdSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task<Product> GetProductAsync(ProductCode productCode)
        {
            return this.Get(productCode,
                            db =>
                            {
                                var parameters = new { product_code = productCode.Value };
                                return db.QueryFirstOrDefaultAsync<ProductRecord>(ProductRepository.GetProductByCodeSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task<Product> GetProductAsync(ProductName productName)
        {
            return this.Get(productName,
                            db =>
                            {
                                var parameters = new { product_name = productName.Value };
                                return db.QueryFirstOrDefaultAsync<ProductRecord>(ProductRepository.GetProductByNameSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task<Product> SaveProductAsync(Product product) => this.Upsert(product);

        /// <inheritdoc />
        protected override string ExtractDisplayKey(Product value) => value.FriendlyString();

        /// <inheritdoc />
        protected override async Task<ProductRecord> UpsertRecordAsync(IDbConnection db, ProductRecord value)
        {
            await db.ExecuteAsync(ProductRepository.UpsertProductSql, value);
            return value;
        }
    }
}
