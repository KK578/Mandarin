namespace Mandarin.Models.Inventory
{
    public class Product
    {
        public Product(string productCode, string productName, string description, decimal? unitPrice)
        {
            this.ProductCode = productCode;
            this.ProductName = productName;
            this.Description = description;
            this.UnitPrice = unitPrice;
        }

        public string ProductCode { get; }
        public string ProductName { get; }
        public string Description { get; }
        public decimal? UnitPrice { get; }
    }
}
