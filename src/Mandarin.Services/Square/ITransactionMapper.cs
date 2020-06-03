using System;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Square
{
    public interface ITransactionMapper
    {
        IObservable<Transaction> MapToTransaction(Order order);
    }
}
