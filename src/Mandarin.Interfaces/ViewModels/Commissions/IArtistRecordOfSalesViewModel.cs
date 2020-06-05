using System.Threading.Tasks;
using Mandarin.Models.Commissions;

namespace Mandarin.ViewModels.Commissions
{
    public interface IArtistRecordOfSalesViewModel : IViewModel
    {
        ArtistSales Commission { get; }
        bool SendInProgress { get; }
        bool SendSuccessful { get; }
        string StatusMessage { get; }
        string EmailAddress { get; set; }
        string CustomMessage { get; set; }
        void ToggleSentFlag();
        Task SendEmailAsync();
    }
}
