using LLama;

namespace LLamaStack.Core
{
    public class LLamaStackModelContext : IDisposable
    {
        private readonly LLamaContext _context;

        public LLamaStackModelContext(LLamaContext context)
        {
            _context = context;
        }

        public LLamaContext LLamaContext => _context;

        public int ContextSize => _context.ContextSize;

        public void LoadState(string filename)
        {
            _context.LoadState(filename);
        }

        public async Task LoadStateAsync(string filename)
        {
            await Task.Run(() => LoadState(filename));
        }

        public void SaveState(string filename)
        {
            _context.SaveState(filename);
        }

        public async Task SaveStateAsync(string filename)
        {
            await Task.Run(() => SaveState(filename));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
