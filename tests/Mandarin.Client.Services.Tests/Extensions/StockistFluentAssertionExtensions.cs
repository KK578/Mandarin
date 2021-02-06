using FluentAssertions.Primitives;
using Mandarin.Stockists;

namespace Mandarin.Client.Services.Tests.Extensions
{
    internal static class StockistFluentAssertionExtensions
    {
        public static void MatchStockist(this ObjectAssertions assertions, Stockist expected)
        {
            assertions.BeEquivalentTo(expected, o => o.IgnoringCyclicReferences());
        }

        public static void MatchStockistIgnoringIds(this ObjectAssertions assertions, Stockist expected)
        {
            assertions.BeEquivalentTo(expected,
                                      o => o.IgnoringCyclicReferences()
                                            .Excluding(x => x.StockistId)
                                            .Excluding(x => x.Details.StockistId)
                                            .Excluding(x => x.Commission.StockistId)
                                            .Excluding(x => x.Commission.CommissionId)
                                            .Excluding(x => x.Commission.InsertedAt));
        }
    }
}
