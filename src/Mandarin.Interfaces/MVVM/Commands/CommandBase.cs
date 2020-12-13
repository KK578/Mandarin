using System;
using System.Threading.Tasks;

namespace Mandarin.MVVM.Commands
{
    /// <summary>
    /// Represents the command that will redirect the user to the login page.
    /// </summary>
    public abstract class CommandBase : PropertyChangedBase, ICommand
    {
        /// <inheritdoc />
        public abstract bool CanExecute { get; }

        /// <inheritdoc />
        public abstract Task ExecuteAsync();

        /// <summary>
        /// Raises PropertyChanged event on <see cref="CanExecute" /> for observers to show new state.
        /// This is an override provided for simpler binding.
        /// </summary>
        /// <param name="ignored">Ignored parameter for simplifying subscriptions.</param>
        protected void UpdateCanExecute(object ignored) => this.UpdateCanExecute();

        /// <summary>
        /// Raises PropertyChanged event on <see cref="CanExecute" /> for observers to show new state.
        /// </summary>
        protected void UpdateCanExecute() => this.OnPropertyChanged(nameof(CommandBase.CanExecute));
    }
}
