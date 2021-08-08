using FluentAssertions.Primitives;
using Mandarin.Commissions;

namespace Mandarin.Tests.Data.Extensions
{
    public static class RecordOfSalesFluentAssertionExtensions
    {
        public static void MatchRecordOfSales(this ObjectAssertions assertions, RecordOfSales recordOfSales)
        {
            assertions.BeEquivalentTo(recordOfSales, o => o.ComparingByMembers<RecordOfSales>().ComparingByMembers<Sale>());
        }
    }
}
