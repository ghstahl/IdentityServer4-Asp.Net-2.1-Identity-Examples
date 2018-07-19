using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4Extras.Configuration.DependencyInjection
{
    public class Decorator<TService>
    {
        public TService Instance { get; set; }

        public Decorator(TService instance)
        {
            Instance = instance;
        }
    }

    public class Decorator<TService, TImpl> : Decorator<TService>
        where TImpl : class, TService
    {
        public Decorator(TImpl instance) : base(instance)
        {
        }
    }

    public class DisposableDecorator<TService> : Decorator<TService>, IDisposable
    {
        public DisposableDecorator(TService instance) : base(instance)
        {
        }

        public void Dispose()
        {
            (Instance as IDisposable)?.Dispose();
        }
    }
}
