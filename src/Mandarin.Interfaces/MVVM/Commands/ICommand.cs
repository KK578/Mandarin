using System.Threading.Tasks;

namespace Mandarin.MVVM.Commands
{
    /// <summary>
    /// Represents an executable command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets a value indicating whether the command can be executed.
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExecuteAsync();
    }
}
