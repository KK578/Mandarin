using FluentAssertions.Primitives;
using Mandarin.Inventory;
using Mandarin.Transactions;

namespace Mandarin.Tests.Data.Extensions
{
    public static class TransactionFluentAssertionExtensions
    {
        public static void MatchTransaction(this ObjectAssertions assertions, Transaction expected)
        {
            assertions.BeEquivalentTo(expected,
                                      o => o.ComparingByMembers<Transaction>()
                                            .ComparingByMembers<Subtransaction>()
                                            .ComparingByMembers<Product>()
                                            .Excluding(x => x.TransactionId));
        }
    }
}
