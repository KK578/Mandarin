using System.Threading.Tasks;

namespace Mandarin.MVVM.ViewModels
{
    /// <summary>
    /// Represents that the implementor is a ViewModel.
    /// </summary>
    public interface IViewModel : IPropertyChangedObservable
    {
        /// <summary>
        /// Allows the <see cref="IViewModel"/> to run any asynchronous startup tasks.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitializeAsync();
    }
}
