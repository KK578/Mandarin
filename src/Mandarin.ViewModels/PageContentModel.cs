using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Represents the backing model for getting content for the public webpages.
    /// </summary>
    internal sealed class PageContentModel
    {
        private readonly MarkdownPipeline markdownPipeline;
        private readonly JToken root;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContentModel"/> class.
        /// </summary>
        /// <param name="markdownPipeline">The markdown pipeline.</param>
        /// <param name="root">The root JSON token for all content sections.</param>
        public PageContentModel(MarkdownPipeline markdownPipeline, JToken root)
        {
            this.markdownPipeline = markdownPipeline;
            this.root = root;
        }

        /// <summary>
        /// Gets a compiled markup string of the provided JSON path.
        /// </summary>
        /// <param name="paths">JSON path elements names to be traversed.</param>
        /// <returns>A compiled markup string at the requested JSON path.</returns>
        public MarkupString GetMarkupString(params string[] paths)
        {
            var markupString = this.Get<string>(paths);
            return new MarkupString(Markdown.ToHtml(markupString, this.markdownPipeline));
        }

        /// <summary>
        /// Gets the values as enumerable of <typeparamref name="T" /> of the provided JSON path.
        /// </summary>
        /// <param name="paths">JSON path element names to be traversed.</param>
        /// <typeparam name="T">Type to deserialize json content as.</typeparam>
        /// <returns>An enumerable of T at the requested JSON path.</returns>
        public IEnumerable<T> GetAll<T>(params string[] paths)
        {
            return this.GetToken(paths).ToObject<IEnumerable<T>>();
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
