using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Common
{
    /// <summary>
    /// Represents a repository of <typeparamref name="TDomain"/>, which is stored in database as <typeparamref name="TRecord"/>.
    /// </summary>
    /// <typeparam name="TDomain">The domain type of the repository.</typeparam>
    /// <typeparam name="TRecord">The record type of the repository.</typeparam>
    internal abstract class DatabaseRepositoryBase<TDomain, TRecord>
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<DatabaseRepositoryBase<TDomain, TRecord>> logger;

        private readonly string typeName = typeof(TDomain).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryBase{TDomain, TRecord}"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        protected DatabaseRepositoryBase(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<DatabaseRepositoryBase<TDomain, TRecord>> logger)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the <typeparamref name="TDomain"/> corresponding to the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="recordFunc">The function to call with the provided key to retrieve rows from the database.</param>
        /// <typeparam name="T">The type of the key to be searched.</typeparam>
        /// <returns>A <see cref="Task"/> containing the <typeparamref name="TDomain"/> for the given <paramref name="key"/>.</returns>
        protected async Task<TDomain> Get<T>(T key, Func<IDbConnection, Task<TRecord>> recordFunc)
        {
            this.logger.LogTrace($"Fetching {this.typeName} entry for Key={{Key}}.", key);

            try
            {
                using var db = this.mandarinDbContext.GetConnection();
                var record = await recordFunc(db);
                var value = this.mapper.Map<TDomain>(record);
                this.logger.LogInformation($"Successfully fetched {this.typeName} entry for Key={{Key}}.", key);
                return value;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to fetch {this.typeName} entry for Key={{Key}}.", key);
                throw;
            }
        }

        /// <summary>
        /// Gets all <typeparamref name="TDomain"/> entries from the given query function.
        /// </summary>
        /// <param name="recordsFunc">The function to call with to retrieve rows from the database.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of <typeparamref name="TDomain"/>.</returns>
        protected async Task<IReadOnlyList<TDomain>> GetAll(Func<IDbConnection, Task<IEnumerable<TRecord>>> recordsFunc)
        {
            this.logger.LogDebug($"Fetching {this.typeName} entries.");

            try
            {
                using var db = this.mandarinDbContext.GetConnection();
                var records = await recordsFunc(db);
                var values = this.mapper.Map<List<TDomain>>(records).AsReadOnly();
                this.logger.LogTrace($"Successfully fetched {{Count}} {this.typeName} entries.", values.Count);
                return values;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to fetch {this.typeName} entries.");
                throw;
            }
        }

        /// <summary>
        /// Inserts or updates the existing database record corresponding to <typeparamref name="TDomain"/>.
        /// </summary>
        /// <param name="value">The domain value to be added/updated.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the new version of the inserted/updated <typeparamref name="TDomain"/>.</returns>
        protected async Task<TDomain> Upsert(TDomain value)
        {
            var key = this.ExtractDisplayKey(value);
            this.logger.LogDebug($"Inserting/Updating {this.typeName} entry for Key={{Key}}: {{@Value}}", key, value);

            try
            {
                var record = this.mapper.Map<TRecord>(value);
                using var db = this.mandarinDbContext.GetConnection();
                var newRecord = await this.UpsertRecordAsync(db, record);
                var newValue = this.mapper.Map<TDomain>(newRecord);
                this.logger.LogTrace($"Successfully saved {this.typeName} entry for Key={{Key}}.", key);
                return newValue;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to save {this.typeName} entry for Key={{Key}}.", key);
                throw;
            }
        }

        /// <summary>
        /// Executes the provided sql, expecting to delete the record.
        /// </summary>
        /// <param name="sql">The sql to delete the record.</param>
        /// <param name="parameters">The sql parameters for deleting the record.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task Delete(string sql, object parameters)
        {
            this.logger.LogDebug($"Deleting {this.typeName} entry for {{Key}}.", parameters);

            try
            {
                using var db = this.mandarinDbContext.GetConnection();
                var affectedRows = await db.ExecuteAsync(sql, parameters);
                this.logger.LogInformation($"Successfully deleted {{Count}} {this.typeName} entry(ies) for Key={{Key}}.", affectedRows, parameters);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to fetch {this.typeName} entry for Key={{Key}}.", parameters);
                throw;
            }
        }

        /// <summary>
        /// Gets the primary key from the given domain object.
        /// </summary>
        /// <param name="value">Domain object to extract the primary key from.</param>
        /// <returns>The primary key of the domain object.</returns>
        protected abstract string ExtractDisplayKey(TDomain value);

        /// <summary>
        /// Inserts or Updates the given record in the database. Returns the new state of the record.
        /// </summary>
        /// <param name="db">The database connection.</param>
        /// <param name="value">The database record to be inserted/updated.</param>
        /// <returns>A <see cref="Task"/> containing the new state of the <typeparamref name="TRecord"/>.</returns>
        protected abstract Task<TRecord> UpsertRecordAsync(IDbConnection db, TRecord value);
    }
}
