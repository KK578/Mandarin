using System;
using Mandarin.Common;
using Mandarin.Stockists;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <inheritdoc cref="IArtistViewModel" />
    internal sealed class ArtistViewModel : ReactiveObject, IArtistViewModel
    {
        private string stockistCode;
        private StatusMode statusCode;
        private string firstName;
        private string lastName;
        private string displayName;
        private string twitterHandle;
        private string instagramHandle;
        private string facebookHandle;
        private string websiteUrl;
        private string tumblrHandle;
        private string emailAddress;
        private DateTime startDate;
        private DateTime endDate;
        private int rate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="stockist">The pre-existing stockist to populate from.</param>
        public ArtistViewModel(Stockist stockist)
        {
            this.stockistCode = stockist.StockistCode;
            this.statusCode = stockist.StatusCode;
            this.firstName = stockist.Details.FirstName;
            this.lastName = stockist.Details.LastName;
            this.displayName = stockist.Details.DisplayName;
            this.twitterHandle = stockist.Details.TwitterHandle;
            this.instagramHandle = stockist.Details.InstagramHandle;
            this.facebookHandle = stockist.Details.FacebookHandle;
            this.websiteUrl = stockist.Details.WebsiteUrl;
            this.tumblrHandle = stockist.Details.TumblrHandle;
            this.emailAddress = stockist.Details.EmailAddress;
            this.startDate = stockist.Commission.StartDate;
            this.endDate = stockist.Commission.EndDate;
            this.rate = stockist.Commission.Rate;
        }

        /// <inheritdoc />
        public string StockistCode
        {
            get => this.stockistCode;
            set => this.RaiseAndSetIfChanged(ref this.stockistCode, value);
        }

        /// <inheritdoc />
        public StatusMode StatusCode
        {
            get => this.statusCode;
            set => this.RaiseAndSetIfChanged(ref this.statusCode, value);
        }

        /// <inheritdoc />
        public string FirstName
        {
            get => this.firstName;
            set => this.RaiseAndSetIfChanged(ref this.firstName, value);
        }

        /// <inheritdoc />
        public string LastName
        {
            get => this.lastName;
            set => this.RaiseAndSetIfChanged(ref this.lastName, value);
        }

        /// <inheritdoc />
        public string DisplayName
        {
            get => this.displayName;
            set => this.RaiseAndSetIfChanged(ref this.displayName, value);
        }

        /// <inheritdoc />
        public string TwitterHandle
        {
            get => this.twitterHandle;
            set => this.RaiseAndSetIfChanged(ref this.twitterHandle, value);
        }

        /// <inheritdoc />
        public string InstagramHandle
        {
            get => this.instagramHandle;
            set => this.RaiseAndSetIfChanged(ref this.instagramHandle, value);
        }

        /// <inheritdoc />
        public string FacebookHandle
        {
            get => this.facebookHandle;
            set => this.RaiseAndSetIfChanged(ref this.facebookHandle, value);
        }

        /// <inheritdoc />
        public string WebsiteUrl
        {
            get => this.websiteUrl;
            set => this.RaiseAndSetIfChanged(ref this.websiteUrl, value);
        }

        /// <inheritdoc />
        public string TumblrHandle
        {
            get => this.tumblrHandle;
            set => this.RaiseAndSetIfChanged(ref this.tumblrHandle, value);
        }

        /// <inheritdoc />
        public string EmailAddress
        {
            get => this.emailAddress;
            set => this.RaiseAndSetIfChanged(ref this.emailAddress, value);
        }

        /// <inheritdoc />
        public DateTime StartDate
        {
            get => this.startDate;
            set => this.RaiseAndSetIfChanged(ref this.startDate, value);
        }

        /// <inheritdoc />
        public DateTime EndDate
        {
            get => this.endDate;
            set => this.RaiseAndSetIfChanged(ref this.endDate, value);
        }

        /// <inheritdoc />
        public int Rate
        {
            get => this.rate;
            set => this.RaiseAndSetIfChanged(ref this.rate, value);
        }
    }
}
