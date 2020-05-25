using System;
using Square.Models;

namespace Mandarin.Services
{
    public interface ITransactionService
    {
        IObservable<Order> GetAllTransactions(DateTime start, DateTime end);
    }
}
