using System.Linq;
using Newtonsoft.Json.Linq;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Represents the backing model for getting content for the public webpages.
    /// </summary>
    internal sealed class PageContentModel
    {
        private readonly JToken root;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContentModel"/> class.
        /// </summary>
        /// <param name="root">The root JSON token for all content sections.</param>
        public PageContentModel(JToken root)
        {
            this.root = root;
        }

        /// <summary>
        /// Gets the value as <typeparamref name="T" /> of the provided JSON path.
        /// </summary>
        /// <param name="paths">JSON path element names to be traversed.</param>
        /// <typeparam name="T">Type to deserialize json content as.</typeparam>
        /// <returns>Instance of T at the requested JSON path.</returns>
        public T Get<T>(params string[] paths)
        {
            return this.GetToken(paths).ToObject<T>();
        }

        private JToken GetToken(params string[] paths)
        {
            return paths.Aggregate(this.root, (current, path) => current?[path]);
        }
    }
}
