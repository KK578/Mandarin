using System.Threading.Tasks;
using Mandarin.MVVM.Commands;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Navigation
{
    /// <summary>
    /// Represents a command that will redirect the user to a new page.
    /// </summary>
    public abstract class RedirectToCommandBase : CommandBase
    {
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToCommandBase"/> class.
        /// </summary>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        protected RedirectToCommandBase(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc />
        public override bool CanExecute => true;

        /// <summary>
        /// Redirects the user's session to login with a return url of the current location.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task ExecuteAsync()
        {
            this.navigationManager.NavigateTo(this.GetTargetUri());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns the new URI that the user should be navigated to.
        /// </summary>
        /// <returns>The URI path to navigate to.</returns>
        protected abstract string GetTargetUri();
    }
}
