using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandarin.Models.Transactions
{
    public class Transaction
    {
        public Transaction(string squareId,
                           decimal totalAmount,
                           DateTime timestamp,
                           string insertedBy,
                           IEnumerable<Subtransaction> subtransactions)
        {
            this.SquareId = squareId;
            this.TotalAmount = totalAmount;
            this.Timestamp = timestamp;
            this.InsertedBy = insertedBy;
            this.Subtransactions = subtransactions.ToList().AsReadOnly();
        }

        public string SquareId { get; }
        public decimal TotalAmount { get; }
        public DateTime Timestamp { get; }
        public string InsertedBy { get; }

        public IReadOnlyList<Subtransaction> Subtransactions { get; }

        public override string ToString()
        {
            return $"{this.Timestamp}: {this.Subtransactions.Count} Transactions = {this.TotalAmount:C}";
        }
    }
}
