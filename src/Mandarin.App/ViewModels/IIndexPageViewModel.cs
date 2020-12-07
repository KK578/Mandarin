using System;
using Blazorise;
using Mandarin.MVVM.ViewModels;

namespace Mandarin.App.ViewModels
{
    /// <summary>
    /// Represents the <see cref="IViewModel"/> for the Index page.
    /// </summary>
    public interface IIndexPageViewModel : IViewModel
    {
        /// <summary>
        /// Gets the icon to be displayed in the header.
        /// </summary>
        IconName Icon { get; }

        /// <summary>
        /// Gets the current version of the application.
        /// </summary>
        Version Version { get; }
    }
}
