using System;
using System.Linq;
using System.Reflection;
using Bashi.Core.Utils;
using Blazorise;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Index
{
    /// <inheritdoc cref="Mandarin.Client.ViewModels.Index.IIndexViewModel" />
    internal sealed class IndexViewModel : ReactiveObject, IIndexViewModel
    {
        private static readonly Random Random = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        public IndexViewModel()
        {
            var choices = EnumUtil.GetValues<IconName>().ToList();
            this.Icon = choices[IndexViewModel.Random.Next(0, choices.Count)];
            this.Version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <inheritdoc/>
        public IconName Icon { get; }

        /// <inheritdoc/>
        public Version Version { get; }
    }
}
