using System;
using System.Linq;
using System.Reflection;
using Bashi.Core.Utils;
using Blazorise;
using Mandarin.MVVM.ViewModels;

namespace Mandarin.App.ViewModels
{
    /// <inheritdoc cref="Mandarin.App.ViewModels.IIndexPageViewModel" />
    internal sealed class IndexPageViewModel : ViewModelBase, IIndexPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPageViewModel"/> class.
        /// </summary>
        public IndexPageViewModel()
        {
            var random = new Random();
            var choices = EnumUtil.GetValues<IconName>().ToList();
            this.Icon = choices[random.Next(0, choices.Count)];
            this.Version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <inheritdoc/>
        public IconName Icon { get; }

        /// <inheritdoc/>
        public Version Version { get; }
    }
}
