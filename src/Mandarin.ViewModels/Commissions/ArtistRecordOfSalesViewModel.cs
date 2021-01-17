﻿using System;
using System.Threading.Tasks;
using Mandarin.Commissions;
using Mandarin.Emails;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mandarin.ViewModels.Commissions
{
    /// <inheritdoc cref="IArtistRecordOfSalesViewModel" />
    internal class ArtistRecordOfSalesViewModel : ViewModelBase, IArtistRecordOfSalesViewModel
    {
        private readonly IEmailService emailService;
        private readonly PageContentModel pageContentModel;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        private bool sendInProgress;
        private bool sendSuccessful;
        private string statusMessage;
        private string emailAddress;
        private string customMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistRecordOfSalesViewModel"/> class.
        /// </summary>
        /// <param name="emailService">The application service for sending emails.</param>
        /// <param name="pageContentModel">The website content model.</param>
        /// <param name="authenticationStateProvider">The service which provides access to the current authentication state..</param>
        /// <param name="recordOfSales">The artist commission breakdown.</param>
        public ArtistRecordOfSalesViewModel(IEmailService emailService, PageContentModel pageContentModel, AuthenticationStateProvider authenticationStateProvider, RecordOfSales recordOfSales)
        {
            this.emailService = emailService;
            this.pageContentModel = pageContentModel;
            this.authenticationStateProvider = authenticationStateProvider;
            this.RecordOfSales = recordOfSales;
        }

        /// <inheritdoc/>
        public RecordOfSales RecordOfSales { get; }

        /// <inheritdoc/>
        public bool SendInProgress { get => this.sendInProgress; private set => this.RaiseAndSetPropertyChanged(ref this.sendInProgress, value); }

        /// <inheritdoc/>
        public bool SendSuccessful { get => this.sendSuccessful; private set => this.RaiseAndSetPropertyChanged(ref this.sendSuccessful, value); }

        /// <inheritdoc/>
        public string StatusMessage { get => this.statusMessage; private set => this.RaiseAndSetPropertyChanged(ref this.statusMessage, value); }

        /// <inheritdoc/>
        public string EmailAddress { get => this.emailAddress; set => this.RaiseAndSetPropertyChanged(ref this.emailAddress, value); }

        /// <inheritdoc/>
        public string CustomMessage { get => this.customMessage; set => this.RaiseAndSetPropertyChanged(ref this.customMessage, value); }

        /// <inheritdoc/>
        public async Task SetMessageFromTemplate(RecordOfSalesTemplateKey templateKey)
        {
            var authenticationState = await this.authenticationStateProvider.GetAuthenticationStateAsync();
            var name = authenticationState?.User?.Identity?.Name;
            var templateFormat = this.pageContentModel.Get<string>("Admin", "RecordOfSales", "Templates", templateKey.ToString());
            this.CustomMessage = string.Format(templateFormat, this.RecordOfSales.FirstName ?? this.RecordOfSales.Name, name);
        }

        /// <inheritdoc/>
        public void ToggleSentFlag()
        {
            if (this.SendSuccessful)
            {
                this.SendSuccessful = false;
                this.StatusMessage = null;
            }
            else
            {
                this.SendSuccessful = true;
                this.StatusMessage = "Ignored.";
            }
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync()
        {
            this.SendInProgress = true;
            this.SendSuccessful = false;
            this.StatusMessage = null;

            try
            {
                var recordOfSales = this.RecordOfSales.WithMessageCustomisations(this.EmailAddress, this.CustomMessage);
                var response = await this.emailService.SendRecordOfSalesEmailAsync(recordOfSales);

                this.SendSuccessful = response.IsSuccess;
                this.StatusMessage = response.Message;
            }
            catch (Exception ex)
            {
                this.SendSuccessful = false;
                this.StatusMessage = ex.Message;
            }
            finally
            {
                this.SendInProgress = false;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.RecordOfSales} to {this.EmailAddress}";
        }
    }
}
