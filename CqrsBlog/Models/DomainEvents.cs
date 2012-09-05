namespace CqrsBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DomainEvents
    {
        private static readonly List<Delegate> Actions = new List<Delegate>();

        private static readonly List<Handler> Handlers = new List<Handler>();

        public static void Register<T>(Action<T> callback)
        {
            Actions.Add(callback);
        }

        public static void RegisterHandler<T>(Func<T> factory)
        {
            Handlers.Add(new Handler<T>(factory));
        }

        public static void Raise<T>(T @event)
        {
            foreach (var action in Actions.OfType<Action<T>>())
            {
                action(@event);
            }

            foreach (var handler in Handlers.Where(x => x.CanHandle<T>()).Select(x => x.CreateInstance<T>()))
            {
                handler.Handle(@event);
            }
        }

        private abstract class Handler
        {
            public abstract bool CanHandle<TEvent>();

            public abstract IHandles<TEvent> CreateInstance<TEvent>();
        }

        private class Handler<T> : Handler
        {
            private readonly Func<T> _factory;

            public Handler(Func<T> factory)
            {
                _factory = factory;
            }

            public override bool CanHandle<TEvent>()
            {
                return typeof(IHandles<TEvent>).IsAssignableFrom(typeof(T));
            }

            public override IHandles<TEvent> CreateInstance<TEvent>()
            {
                return (IHandles<TEvent>)_factory();
            }
        }
    }
}