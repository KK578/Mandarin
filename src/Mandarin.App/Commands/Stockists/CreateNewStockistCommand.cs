using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents a command to direct the user to create a new <see cref="Stockist"/>.
    /// </summary>
    internal sealed class CreateNewStockistCommand : CommandBase
    {
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewStockistCommand"/> class.
        /// </summary>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public CreateNewStockistCommand(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc/>
        public override bool CanExecute => true;

        /// <inheritdoc/>
        public override Task ExecuteAsync()
        {
            this.navigationManager.NavigateTo("/stockists/new");
            return Task.CompletedTask;
        }
    }
}
