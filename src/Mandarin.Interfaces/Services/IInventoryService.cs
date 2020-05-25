using System;
using Square.Models;

namespace Mandarin.Services
{
    public interface IInventoryService
    {
        IObservable<CatalogObject> GetInventory();
    }
}
