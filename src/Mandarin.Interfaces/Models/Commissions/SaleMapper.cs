using Mandarin.Models.Transactions;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Maps objects to <see cref="Sale"/>.
    /// </summary>
    public static class SaleMapper
    {
        /// <summary>
        /// Maps a <see cref="Subtransaction"/> to a <see cref="Sale"/>.
        /// </summary>
        /// <param name="subtransaction">The subtransactions to be mapped.</param>
        /// <param name="rate">The commission rate to apply to the subtransactions.</param>
        /// <param name="fixedCommission">Fixed commission amount to apply to the item.</param>
        /// <returns>Mapped <see cref="Sale"/> object.</returns>
        public static Sale FromTransaction(Subtransaction subtransaction, decimal rate, FixedCommissionAmount fixedCommission)
        {
            var fixedAmount = fixedCommission?.Amount ?? 0;
            var subTotal = subtransaction.Subtotal - (fixedAmount * subtransaction.Quantity);
            var commission = subTotal * rate;
            var sale = new Sale(subtransaction.Product.ProductCode,
                                subtransaction.Product.ProductName,
                                subtransaction.Quantity,
                                subtransaction.TransactionUnitPrice - fixedAmount,
                                subTotal,
                                -commission,
                                subTotal - commission);

            return sale;
        }
    }
}
