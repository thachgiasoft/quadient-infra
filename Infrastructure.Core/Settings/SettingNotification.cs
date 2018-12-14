using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Core.Settings
{
    public class SettingNotification : IObservable<SettingNotify>
    {
        private readonly List<IObserver<SettingNotify>> _observers;
        public SettingNotification()
        {
            _observers = new List<IObserver<SettingNotify>>();
        }
        public IDisposable Subscribe(IObserver<SettingNotify> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }
        public void NotifyForSettingChange(SettingNotify notify)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(notify);
            }
        }
    }

    class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<SettingNotify>> _observers;
        private readonly IObserver<SettingNotify> _observer;

        public Unsubscriber(List<IObserver<SettingNotify>> observers, IObserver<SettingNotify> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
