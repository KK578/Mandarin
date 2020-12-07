﻿using System;
using System.Threading.Tasks;
using Mandarin.MVVM.Commands;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Navigation
{
    /// <summary>
    /// Represents the command that will redirect the user to the login page.
    /// </summary>
    public class RedirectToLoginCommand : ICommand
    {
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToLoginCommand"/> class.
        /// </summary>
        /// <param name="navigationManager">The application instance for URI navigation management.</param>
        public RedirectToLoginCommand(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        /// <summary>
        /// Redirects the user's session to login with a return url of the current location.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ExecuteAsync()
        {
            var returnUrl = Uri.EscapeDataString(this.navigationManager.Uri);
            this.navigationManager.NavigateTo($"authentication/login?returnUrl={returnUrl}");

            return Task.CompletedTask;
        }
    }
}
