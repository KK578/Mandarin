using Square.Models;

namespace Mandarin.Services.Transactions.External
{
    /// <summary>
    /// Represents extensions on <see cref="Money"/>.
    /// </summary>
    internal static class MoneyExtensions
    {
        /// <summary>
        /// Converts the given <see cref="Money"/> to a decimal representative of GBP.
        /// </summary>
        /// <param name="money">The Square monetary amount.</param>
        /// <returns>The monetary amount adjusted as GBP.</returns>
        public static decimal ToDecimal(this Money money) => decimal.Divide(money.Amount.GetValueOrDefault(), 100);
    }
}
