using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Core.Pause
{
    /// <summary>
    /// 등록된 IPausable들을 HashSet으로 관리하는 시스템.
    /// Pause가 불러지면 등록된 게임오브젝트들의 Pause 메소드가 실행된다.
    /// </summary>
    public static class PauseManager
    {
        public static bool IsSuperPaused { get; private set; } = false; // 최상단의 일시정지가 수행되었는지를 관리 (ex. pauseMenu 등 게임 로직과 무관한 일시정지)
        public static bool IsPaused { get; private set; } = false;


        private static HashSet<IPausable> pausables = new HashSet<IPausable>(1024);

        public static void Register(IPausable pausable)
        {
            pausables.Add(pausable);
        }

        public static void Unregister(IPausable pausable)
        {
            pausables.Remove(pausable);
        }

        public static void Pause(bool isSuperPause = false)
        {
            if (isSuperPause == true)
            {
                IsSuperPaused = true;
            }
            else
            {
                IsPaused = true;
            }

            foreach (IPausable pausable in pausables)
            {
                pausable.Pause();
            }
        }

        public static void Resume(bool isSuperPause = false)
        {
            // 게임이 멈춰있지 않으면 그대로 진행
            if (IsPaused == false && IsSuperPaused == false)
            {
                return;
            }

            if (isSuperPause == true)
            {
                IsSuperPaused = false;
            }
            else
            {
                IsPaused = false;
            }

            if (IsPaused == false && IsSuperPaused == false)
            {
                foreach (IPausable pausable in pausables)
                {
                    pausable.Resume();
                }
            }
        }
    }
}