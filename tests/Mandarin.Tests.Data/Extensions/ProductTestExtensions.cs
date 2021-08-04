using Mandarin.Inventory;

namespace Mandarin.Tests.Data.Extensions
{
    public static class ProductTestExtensions
    {
        public static Product WithTlmProductCode(this Product product)
        {
            return product with
            {
                ProductCode = ProductCode.Of($"TLM-{product.ProductCode}"),
            };
        }
    }
}
