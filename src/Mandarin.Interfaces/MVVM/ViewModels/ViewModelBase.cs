using System.Threading.Tasks;

namespace Mandarin.MVVM.ViewModels
{
    /// <summary>
    /// Implements the basic functionality for all <see cref="IViewModel"/> classes.
    /// </summary>
    public abstract class ViewModelBase : PropertyChangedBase, IViewModel
    {
        /// <inheritdoc />
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
