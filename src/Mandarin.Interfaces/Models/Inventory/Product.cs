namespace Mandarin.Models.Inventory
{
    public class Product
    {
        public Product(string squareId, string productCode, string productName, string description, decimal? unitPrice)
        {
            this.SquareId = squareId;
            this.ProductCode = productCode;
            this.ProductName = productName;
            this.Description = description;
            this.UnitPrice = unitPrice;
        }

        public string SquareId { get; }
        public string ProductCode { get; }
        public string ProductName { get; }
        public string Description { get; }
        public decimal? UnitPrice { get; }
    }
}
