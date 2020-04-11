using System;
using System.Threading.Tasks;
using Mandarin.Models.Contact;

namespace Mandarin.ViewModels.Contact
{
    public interface IContactPageViewModel
    {
        ContactDetailsModel Model { get; }
        bool LastSubmitSuccessful { get; }
        Exception SubmitException { get; }

        Task SubmitAsync();
    }
}
