using System;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Navigation
{
    /// <summary>
    /// Represents the command that will redirect the user to the login page.
    /// </summary>
    public class RedirectToLoginCommand : RedirectToCommandBase
    {
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToLoginCommand"/> class.
        /// </summary>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public RedirectToLoginCommand(NavigationManager navigationManager)
            : base(navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc />
        protected override string GetTargetUri()
        {
            var returnUrl = Uri.EscapeDataString(this.navigationManager.Uri);
            return $"authentication/login?returnUrl={returnUrl}";
        }
    }
}
