using System;
using Mandarin.Models.Commissions;

namespace Mandarin.Services
{
    public interface ICommissionService
    {
        IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end);
    }
}
