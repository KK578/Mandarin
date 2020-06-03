using System;
using System.Threading.Tasks;
using Mandarin.Models.Inventory;

namespace Mandarin.Services
{
    public interface IQueryableInventoryService : IInventoryService
    {
        Task<Product> GetProductByIdAsync(string squareId);
        Task<Product> GetProductByNameAsync(string productName);
    }

    public interface IInventoryService
    {
        IObservable<Product> GetInventory();
    }
}
