namespace FoxHill.Core.Settings
{
    public abstract class SettingsBase
    {
        public abstract void OnExitSettingSelection(); // 해당 Setting Selection을 떠날 때 동작
        public abstract void OnSettingClosed(); // UI가 꺼질 때 필요한 부분 초기화 로직
    }
}
