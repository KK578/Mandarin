using FluentAssertions.Equivalency;
using FluentAssertions.Primitives;
using Mandarin.Commissions;
using Mandarin.Stockists;

namespace Mandarin.Tests.Data.Extensions
{
    public static class StockistFluentAssertionExtensions
    {
        public static void MatchStockist(this ObjectAssertions assertions, Stockist expected)
        {
            assertions.BeEquivalentTo(expected, o => o.CompareByStockistMembers());
        }

        public static void MatchStockistIgnoringIds(this ObjectAssertions assertions, Stockist expected)
        {
            assertions.BeEquivalentTo(expected,
                                      o => o.CompareByStockistMembers()
                                            .Excluding(x => x.StockistId)
                                            .Excluding(x => x.Details.StockistId)
                                            .Excluding(x => x.Commission.StockistId)
                                            .Excluding(x => x.Commission.CommissionId));
        }

        private static EquivalencyAssertionOptions<Stockist> CompareByStockistMembers(this EquivalencyAssertionOptions<Stockist> options)
        {
            return options.ComparingByMembers<Stockist>()
                          .ComparingByMembers<StockistDetail>()
                          .ComparingByMembers<Commission>()
                          .Excluding(x => x.Commission.InsertedAt);
        }
    }
}
