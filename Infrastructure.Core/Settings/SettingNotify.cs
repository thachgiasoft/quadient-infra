namespace Infrastructure.Core.Settings
{
    public class SettingNotify
    {
        public SettingNotify(Setting setting)
        {
            Setting = setting;
        }

        public Setting Setting { get; set; }
        public string NotifyType { get; set; }
    }
}
