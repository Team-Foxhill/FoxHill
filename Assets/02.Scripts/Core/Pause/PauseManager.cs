using System.Collections.Generic;

namespace FoxHill.Core.Pause
{
    /// <summary>
    /// 등록된 IPausable들을 HashSet으로 관리하는 시스템.
    /// Pause가 불러지면 등록된 게임오브젝트들의 Pause 메소드가 실행된다.
    /// </summary>
    public static class PauseManager
    {
        public static bool IsPaused => _isPaused; // 가급적 사용하지 말 것.

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