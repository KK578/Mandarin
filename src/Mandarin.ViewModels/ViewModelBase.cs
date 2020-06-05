using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Mandarin.ViewModels
{
    internal abstract class ViewModelBase : IViewModel
    {
        private readonly ISubject<string> stateSubject;

        protected ViewModelBase()
        {
            this.stateSubject = new Subject<string>();
        }

        public IObservable<string> StateObservable => this.stateSubject.AsObservable();

        protected void RaiseAndSetPropertyChanged<T>(ref T property, T value, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                this.OnPropertyChanged(name);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) => this.stateSubject.OnNext(name);
    }
}
