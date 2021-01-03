using System.Text.RegularExpressions;
using FluentAssertions.Primitives;
using Mandarin.Models.Stockists;

namespace Mandarin.Tests.Extensions
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
                                            .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, @"Commissions\[0\].StockistId"))
                                            .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, @"Commissions\[0\].CommissionId"))
                                            .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, @"Commissions\[0\].InsertedAt")));
        }
    }
}
