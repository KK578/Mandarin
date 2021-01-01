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
        /// <returns>Mapped <see cref="Sale"/> object.</returns>
        public static Sale FromTransaction(Subtransaction subtransaction, decimal rate)
        {
            var subTotal = subtransaction.Subtotal;
            var commission = subTotal * rate;
            var sale = new Sale
            {
                ProductCode = subtransaction.Product.ProductCode,
                ProductName = subtransaction.Product.ProductName,
                Quantity = subtransaction.Quantity,
                UnitPrice = subtransaction.TransactionUnitPrice,
                Subtotal = subTotal,
                Commission = -commission,
                Total = subTotal - commission,
            };

            return sale;
        }
    }
}
