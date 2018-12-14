namespace Infrastructure.Core.Settings
{
    public class TestSettingObserver : BaseSettingObserver
    {
        public override string Key { get { return "TestSettingKey"; } }
        public override void Notify(SettingNotify value)
        {
            //Islemler.
        }
    }
}
