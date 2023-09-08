using LLama;

namespace LLamaStack.Core
{
    /// <summary>
    /// Wrapper class for LLamaSharp LLamaContext
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LLamaStackContext : IDisposable
    {
        private readonly LLamaContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaStackContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LLamaStackContext(LLamaContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Gets the LLamaSharp context.
        /// </summary>
        public LLamaContext LLamaContext => _context;


        /// <summary>
        /// Gets the size of the context.
        /// </summary>
        public int ContextSize => _context.ContextSize;


        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void LoadState(string filename)
        {
            _context.LoadState(filename);
        }


        /// <summary>
        /// Loads the state asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public async Task LoadStateAsync(string filename)
        {
            await Task.Run(() => LoadState(filename));
        }


        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveState(string filename)
        {
            _context.SaveState(filename);
        }


        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public async Task SaveStateAsync(string filename)
        {
            await Task.Run(() => SaveState(filename));
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
