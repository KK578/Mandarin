using System;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services;

namespace Mandarin.ViewModels.Commissions
{
    /// <inheritdoc />
    internal class ArtistRecordOfSalesViewModel : ViewModelBase, IArtistRecordOfSalesViewModel
    {
        private readonly IEmailService emailService;
        private bool sendInProgress;
        private bool sendSuccessful;
        private string statusMessage;
        private string emailAddress;
        private string customMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistRecordOfSalesViewModel"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="commission">The artist commission breakdown.</param>
        public ArtistRecordOfSalesViewModel(IEmailService emailService, ArtistSales commission)
        {
            this.emailService = emailService;
            this.Commission = commission;
        }

        /// <inheritdoc/>
        public ArtistSales Commission { get; }

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
                var commission = this.Commission.WithMessageCustomisations(this.EmailAddress, this.CustomMessage);
                var email = this.emailService.BuildRecordOfSalesEmail(commission);
                await this.emailService.SendEmailAsync(email);

                this.SendSuccessful = true;
                this.StatusMessage = $"Successfully sent to {this.EmailAddress ?? this.Commission.EmailAddress}.";
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
            return $"{this.Commission} to {this.EmailAddress}";
        }
    }
}
