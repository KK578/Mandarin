using System;

namespace Mandarin.ViewModels
{
    public interface IViewModel
    {
        IObservable<string> StateObservable { get; }
    }
}
