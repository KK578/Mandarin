using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.App.Commands.Stockists;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <inheritdoc cref="IStockistsNewPageViewModel" />
    internal sealed class StockistsNewPageViewModel : ViewModelBase, IStockistsNewPageViewModel
    {
        private readonly ICommissionService commissionService;

        private Stockist selectedStockist;
        private IReadOnlyList<CommissionRateGroup> commissionRateGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsNewPageViewModel"/> class.
        /// </summary>
        /// <param name="commissionService">The service for interacting with commission details.</param>
        /// <param name="redirectToStockistsIndexCommand">The command to redirect the user to the stockists index page.</param>
        /// <param name="saveNewStockistCommand">The command to save the newly created stockist.</param>
        public StockistsNewPageViewModel(ICommissionService commissionService,
                                         RedirectToStockistsIndexCommand redirectToStockistsIndexCommand,
                                         SaveNewStockistCommand saveNewStockistCommand)
        {
            this.commissionService = commissionService;
            this.CloseCommand = redirectToStockistsIndexCommand;
            this.SubmitCommand = saveNewStockistCommand;
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

        /// <inheritdoc/>
        public ICommand CloseCommand { get; }

        /// <inheritdoc/>
        public ICommand SubmitCommand { get; }

        /// <inheritdoc/>
        protected override async Task DoInitializeAsync()
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
