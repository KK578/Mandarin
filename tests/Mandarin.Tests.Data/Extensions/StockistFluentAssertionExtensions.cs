using FluentAssertions.Primitives;
using Mandarin.Stockists;

namespace Mandarin.Tests.Data.Extensions
{
    public static class StockistFluentAssertionExtensions
    {
        public static void MatchStockistIgnoringIds(this ObjectAssertions assertions, Stockist expected)
        {
            assertions.BeEquivalentTo(expected,
                                      o => o.Excluding(x => x.Commission.InsertedAt)
                                            .Excluding(x => x.StockistId)
                                            .Excluding(x => x.Details.StockistId)
                                            .Excluding(x => x.Commission.StockistId)
                                            .Excluding(x => x.Commission.CommissionId));
        }
    }
}
