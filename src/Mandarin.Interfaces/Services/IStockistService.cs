﻿using System;
using System.Threading.Tasks;
using Mandarin.Models.Stockists;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can retrieve and update details about stockists.
    /// </summary>
    public interface IStockistService
    {
        /// <summary>
        /// Gets the specified artist by their unique stockist code.
        /// </summary>
        /// <param name="stockistCode">The stockist code of the stockist.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous fetch operation of the requested stockist.</returns>
        Task<Stockist> GetStockistByCodeAsync(string stockistCode);

        /// <summary>
        /// Gets a list of all known stockists.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing all stockists.</returns>
        public IObservable<Stockist> GetStockistsAsync();

        /// <summary>
        /// Saves all changes made to the stockist. Will automatically detect if they are a new stockist and create them as required.
        /// </summary>
        /// <param name="stockist">The stockist to be added or updated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        Task SaveStockistAsync(Stockist stockist);
    }
}