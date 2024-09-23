using System.Collections.Generic;

namespace FoxHill.Core.Pause
{
    public static class PauseManager
    {
        public static bool IsPaused => _isPaused;

        private static HashSet<IPausable> pausables = new HashSet<IPausable>(1024);
        private static bool _isPaused = false;

        public static void Register(IPausable pausable)
        {
            pausables.Add(pausable);
        }

        public static void Unregister(IPausable pausable)
        {
            pausables.Remove(pausable);
        }

        public static void Pause()
        {
            _isPaused = true;

            foreach (IPausable pausable in pausables)
            {
                pausable.Pause();
            }
        }

        public static void Resume()
        {
            _isPaused = false;

            foreach (IPausable pausable in pausables)
            {
                pausable.Resume();
            }
        }
    }
}