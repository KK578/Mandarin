using System;
using Mandarin.Models.Inventory;

namespace Mandarin.Services
{
    public interface IInventoryService
    {
        IObservable<Product> GetInventory();
    }
}
