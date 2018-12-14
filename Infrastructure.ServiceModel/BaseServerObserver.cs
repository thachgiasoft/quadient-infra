using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.ServiceModel
{
    public abstract class BaseServerObserver : IObserver<ServerNotify>
    {
        public void OnNext(ServerNotify value)
        {
            Notify(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
        public abstract void Notify(ServerNotify value);
    }
}
