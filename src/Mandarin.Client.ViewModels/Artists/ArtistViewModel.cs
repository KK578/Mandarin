using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Stockists;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <inheritdoc cref="IArtistViewModel" />
    internal sealed class ArtistViewModel : ReactiveObject, IArtistViewModel
    {
        private readonly IClock clock;

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
        private LocalDate startDate;
        private LocalDate endDate;
        private int rate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="stockist">The pre-existing stockist to populate from.</param>
        /// <param name="clock">The application clock instance.</param>
        public ArtistViewModel(Stockist stockist, IClock clock)
        {
            this.clock = clock;
            this.StockistId = stockist.StockistId?.Value;
            this.stockistCode = stockist.StockistCode?.Value;
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
            this.CommissionId = stockist.Commission.CommissionId?.Value;
            this.startDate = stockist.Commission.StartDate;
            this.endDate = stockist.Commission.EndDate;
            this.rate = stockist.Commission.Rate;
        }

        /// <inheritdoc />
        public int? StockistId { get; }

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

        /// <inheritdoc/>
        public int? CommissionId { get; }

        /// <inheritdoc />
        public LocalDate StartDate
        {
            get => this.startDate;
            set => this.RaiseAndSetIfChanged(ref this.startDate, value);
        }

        /// <inheritdoc />
        public LocalDate EndDate
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

        /// <inheritdoc/>
        public Stockist ToStockist()
        {
            var stockistId = Stockists.StockistId.Of(this.StockistId);
            var commissionId = Mandarin.Commissions.CommissionId.Of(this.CommissionId);

            return new Stockist
            {
                StockistId = stockistId,
                StockistCode = Stockists.StockistCode.Of(this.StockistCode),
                StatusCode = this.StatusCode,
                Details = new StockistDetail
                {
                    StockistId = stockistId,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    DisplayName = this.DisplayName,
                    TwitterHandle = this.TwitterHandle,
                    InstagramHandle = this.InstagramHandle,
                    FacebookHandle = this.FacebookHandle,
                    WebsiteUrl = this.WebsiteUrl,
                    TumblrHandle = this.TumblrHandle,
                    EmailAddress = this.EmailAddress,
                },
                Commission = new Commission
                {
                    CommissionId = commissionId,
                    StockistId = stockistId,
                    Rate = this.Rate,
                    StartDate = this.StartDate,
                    EndDate = this.EndDate,
                    InsertedAt = this.clock.GetCurrentInstant(),
                },
            };
        }
    }
}
