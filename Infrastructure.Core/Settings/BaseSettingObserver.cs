using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Settings
{
    public abstract class BaseSettingObserver : IObserver<SettingNotify>
    {
        public void OnNext(SettingNotify value)
        {
            if (value.Setting.Key.Equals(Key, StringComparison.InvariantCulture))
                Notify(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
        public abstract string Key { get; }
        public abstract void Notify(SettingNotify value);
    }
}
