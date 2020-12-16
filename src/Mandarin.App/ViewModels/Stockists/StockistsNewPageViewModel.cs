using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <inheritdoc cref="IStockistsNewPageViewModel" />
    internal sealed class StockistsNewPageViewModel : ViewModelBase, IStockistsNewPageViewModel
    {
        private readonly ICommissionService commissionService;

        private IReadOnlyList<CommissionRateGroup> commissionRateGroups;
        private Stockist selectedStockist;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsNewPageViewModel"/> class.
        /// </summary>
        /// <param name="commissionService">The service for interacting with commission details.</param>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public StockistsNewPageViewModel(ICommissionService commissionService, NavigationManager navigationManager)
        {
            this.commissionService = commissionService;
        }

        /// <inheritdoc/>
        public Stockist Stockist
        {
            get => this.selectedStockist;
            private set => this.RaiseAndSetPropertyChanged(ref this.selectedStockist, value);
        }

        /// <inheritdoc/>
        public IReadOnlyList<CommissionRateGroup> CommissionRateGroups
        {
            get => this.commissionRateGroups;
            private set => this.RaiseAndSetPropertyChanged(ref this.commissionRateGroups, value);
        }

        /// <inheritdoc cref="IViewModel" />
        public override async Task InitializeAsync()
        {
            this.CommissionRateGroups = await this.commissionService.GetCommissionRateGroups();

            this.Stockist = new Stockist
            {
                StatusCode = StatusMode.Active,
                Details = new StockistDetail(),
                Commissions = new List<Commission>
                {
                    new()
                    {
                        RateGroupId = this.commissionRateGroups[0].GroupId,
                        RateGroup = this.commissionRateGroups[0],
                        StartDate = DateTime.Now.Date,
                        EndDate = DateTime.Now.AddDays(90).Date,
                    },
                },
            };
        }
    }
}
