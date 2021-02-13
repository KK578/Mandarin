using System;
using Blazorise;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Index
{
    /// <summary>
    /// Represents the ViewModel for the Index page.
    /// </summary>
    public interface IIndexViewModel : IReactiveObject
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
