using Mandarin.Models.Inventory;

namespace Mandarin.Models.Transactions
{
    public class Subtransaction
    {
        public Subtransaction(Product product, int quantity, decimal subtotal)
        {
            this.Product = product;
            this.Quantity = quantity;
            this.Subtotal = subtotal;
        }

        public Product Product { get; }
        public int Quantity { get; }
        public decimal TransactionUnitPrice => this.Subtotal / this.Quantity;
        public decimal Subtotal { get; }
    }
}
