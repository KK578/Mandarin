using System.Threading.Tasks;

namespace Mandarin.MVVM.ViewModels
{
    /// <summary>
    /// Implements the basic functionality for all <see cref="IViewModel"/> classes.
    /// </summary>
    public abstract class ViewModelBase : PropertyChangedBase, IViewModel
    {
        private bool isLoading = true;

        /// <inheritdoc/>
        public bool IsLoading
        {
            get => this.isLoading;
            private set => this.RaiseAndSetPropertyChanged(ref this.isLoading, value);
        }

        /// <inheritdoc />
        public virtual async Task InitializeAsync()
        {
            try
            {
                this.IsLoading = true;
                await this.DoInitializeAsync();
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Allows the <see cref="IViewModel"/> to run any asynchronous startup tasks.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task DoInitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
