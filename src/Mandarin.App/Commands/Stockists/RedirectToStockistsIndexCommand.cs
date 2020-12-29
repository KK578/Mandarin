using Mandarin.App.Commands.Navigation;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents the command that will redirect the user to the /stockists page.
    /// </summary>
    public class RedirectToStockistsIndexCommand : RedirectToCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToStockistsIndexCommand"/> class.
        /// </summary>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public RedirectToStockistsIndexCommand(NavigationManager navigationManager)
            : base(navigationManager)
        {
        }

        /// <inheritdoc />
        protected override string GetTargetUri() => "/stockists";
    }
}
