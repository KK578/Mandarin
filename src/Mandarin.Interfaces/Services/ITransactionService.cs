using System;
using Mandarin.Models.Transactions;

namespace Mandarin.Services
{
    public interface ITransactionService
    {
        IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end);
    }
}
