using System;
using System.Collections.Generic;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.ServiceModel
{
    public class ServerNotification
    {
        private readonly List<IObserver<ServerNotify>> _observers;
        private bool _isInitialized;
        public ServerNotification()
        {
            _observers = new List<IObserver<ServerNotify>>();
            _isInitialized = false;
        }
        public IDisposable Subscribe(IObserver<ServerNotify> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }
        public void NotifyForServerChanges(ServerNotify notify)
        {
            if (!_isInitialized)
                Initialize();
            foreach (var observer in _observers)
            {
                observer.OnNext(notify);
            }
        }
        private void Initialize()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            foreach (var serverObserver in typeFinder.FindClassesOfType<BaseServerObserver>())
            {
                var type = Activator.CreateInstance(serverObserver) as BaseServerObserver;
                if (type != null)
                    Subscribe(type);
            }
            _isInitialized = true;
        }
    }

    class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<ServerNotify>> _observers;
        private readonly IObserver<ServerNotify> _observer;

        public Unsubscriber(List<IObserver<ServerNotify>> observers, IObserver<ServerNotify> observer)
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
