using Mandarin.App.Commands.Navigation;
using Mandarin.Models.Artists;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents a command to direct the user to create a new <see cref="Stockist"/>.
    /// </summary>
    internal sealed class RedirectToStockistsNewCommand : RedirectToCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToStockistsNewCommand"/> class.
        /// </summary>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public RedirectToStockistsNewCommand(NavigationManager navigationManager)
            : base(navigationManager)
        {
        }

        /// <inheritdoc/>
        protected override string GetTargetUri()
        {
            return "/stockists/new";
        }
    }
}
