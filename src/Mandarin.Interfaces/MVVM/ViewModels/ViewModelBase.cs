using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Mandarin.MVVM.ViewModels
{
    /// <summary>
    /// Implements the basic functionality for all <see cref="IViewModel"/> classes.
    /// </summary>
    public abstract class ViewModelBase : IViewModel
    {
        private readonly ISubject<string> stateSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
            this.stateSubject = new Subject<string>();
        }

        /// <inheritdoc/>
        public IObservable<string> StateObservable => this.stateSubject.AsObservable();

        /// <summary>
        /// Handles an equality check between <paramref name="property"/> and <paramref name="value"/>.
        /// If they are not equal to each other, then pushes a PropertyChanged event into the <see cref="StateObservable"/>.
        /// </summary>
        /// <param name="property">Reference to the property being changed.</param>
        /// <param name="value">New value for the property.</param>
        /// <param name="name">Property name to raise event against.</param>
        /// <typeparam name="T">Type of <paramref name="property"/>.</typeparam>
        protected void RaiseAndSetPropertyChanged<T>(ref T property, T value, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                this.OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// Pushes a PropertyChanged event into the <see cref="StateObservable"/>.
        /// </summary>
        /// <param name="name">Property name to raise event against.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null) => this.stateSubject.OnNext(name);
    }
}
