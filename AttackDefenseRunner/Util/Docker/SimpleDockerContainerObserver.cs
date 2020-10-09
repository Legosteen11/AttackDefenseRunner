using System;
using System.Collections.Generic;
using ContainerList = System.Collections.Generic.List<AttackDefenseRunner.Model.DockerContainer>;

namespace AttackDefenseRunner.Util.Docker
{
    public class SimpleDockerContainerObserver : IDockerContainerObserver
    {
        private readonly List<IObserver<ContainerList>> _observers = new List<IObserver<ContainerList>>();
        
        public void OnCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var observer in _observers)
            {
                observer.OnError(error);
            }
        }

        public void OnNext(ContainerList value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }
        
        public void Subscribe(IObservable<ContainerList> provider)
        {
            provider.Subscribe(this);
        }

        public IDisposable Subscribe(IObserver<ContainerList> observer)
        {
            if (!_observers.Contains(observer))
            {
                // Does not contain observer yet
                _observers.Add(observer);
            }
            return new Unsubscriber(_observers, observer);
        }
    }
    
    class Unsubscriber : IDisposable
    {
        private List<IObserver<ContainerList>>_observers;
        private IObserver<ContainerList> _observer;

        public Unsubscriber(List<IObserver<ContainerList>> observers, IObserver<ContainerList> observer)
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