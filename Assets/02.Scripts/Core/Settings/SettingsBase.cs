using System;

namespace FoxHill.Core.Settings
{
    public abstract class SettingsBase
    {
        public abstract void OnExit(); // UI가 꺼질 때 필요한 부분 초기화 로직
    }
}
