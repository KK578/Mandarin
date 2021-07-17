using Mandarin.Inventory;

namespace Mandarin.Tests.Data.Extensions
{
    public static class ProductTestExtensions
    {
        public static Product WithTlmProductCode(this Product product)
        {
            return product with
            {
                ProductCode = new ProductCode($"TLM-{product.ProductCode}"),
            };
        }
    }
}
