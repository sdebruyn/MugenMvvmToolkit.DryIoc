using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DryIoc;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.IoC;

namespace MugenMvvmToolkit.DryIoc
{
    public class DryIocContainer : IIocContainer
    {
        static int _idCounter;

        readonly IContainer _container;
        readonly DryIocContainer _parent;
        bool _disposed;

        public DryIocContainer()
        {
            _container = new Container(Rules.Default.With(FactoryMethod.ConstructorWithResolvableArguments).WithUnknownServiceResolvers());
            Id = Interlocked.Increment(ref _idCounter);
        }

        DryIocContainer(DryIocContainer parent)
        {
            _parent = parent;
            _container = ((Container)parent.Container).WithRegistrationsCopy();
            Id = Interlocked.Increment(ref _idCounter);
        }

        public void Dispose()
        {
            if (!IsDisposed)
                Disposed?.Invoke(this, EventArgs.Empty);

            _container.Dispose();
            _disposed = true;
        }

        public bool IsDisposed
        {
            get
            {
                if (_container is Container container)
                {
                    return container.IsDisposed;
                }
                return _disposed;
            }
        }

        public event EventHandler<IDisposableObject, EventArgs> Disposed;

        public IIocContainer CreateChild()
        {
            return new DryIocContainer(this);
        }

        public object Get(Type service, string name = null, params IIocParameter[] parameters)
        {
            return _container.Resolve(service, name);
        }

        public IEnumerable<object> GetAll(Type service, string name = null, params IIocParameter[] parameters)
        {
            return _container.ResolveMany(service, serviceKey: name);
        }

        public void BindToConstant(Type service, object instance, string name = null)
        {
            _container.RegisterInstance(service, instance, serviceKey: name);
        }

        public void BindToMethod(Type service, Func<IIocContainer, IList<IIocParameter>, object> methodBindingDelegate,
            DependencyLifecycle lifecycle, string name = null,
            params IIocParameter[] parameters)
        {
            _container.RegisterDelegate(service,
                resolver => methodBindingDelegate(new ResolverBasedContainer(resolver, this), parameters),
                ConvertToReuse(lifecycle), serviceKey: name);
        }

        public void Bind(Type service, Type typeTo, DependencyLifecycle lifecycle, string name = null,
            params IIocParameter[] parameters)
        {
            _container.Register(service, typeTo, ConvertToReuse(lifecycle), serviceKey: name);
        }

        public void Unbind(Type service)
        {
            _container.Unregister(service);
        }

        public bool CanResolve(Type service, string name = null)
        {
            return _container.IsRegistered(service, name);
        }

        public int Id { get; }

        public IIocContainer Parent => _parent;

        public object Container => _container;

        public object GetService(Type serviceType)
        {
            return Get(serviceType);
        }

        static IReuse ConvertToReuse(DependencyLifecycle lifecycle)
        {
            if (lifecycle == DependencyLifecycle.SingleInstance)
                return Reuse.Singleton;

            if (lifecycle == DependencyLifecycle.TransientInstance)
                return Reuse.Transient;

            return null;
        }

        class ResolverBasedContainer : IIocContainer
        {
            readonly DryIocContainer _parentContainer;
            readonly IResolver _resolver;

            public ResolverBasedContainer(IResolver resolver, DryIocContainer parentContainer)
            {
                _resolver = resolver;
                _parentContainer = parentContainer;

                Id = Interlocked.Increment(ref _idCounter);
            }

            public void Dispose()
            {
                if (!IsDisposed)
                    Disposed?.Invoke(this, EventArgs.Empty);

                _parentContainer.Dispose();
            }

            public bool IsDisposed => _parentContainer.IsDisposed;

            public event EventHandler<IDisposableObject, EventArgs> Disposed;

            public IIocContainer CreateChild()
            {
                return new DryIocContainer(_parentContainer);
            }

            public object Get(Type service, string name = null, params IIocParameter[] parameters)
            {
                return _resolver.Resolve(service, name);
            }

            public IEnumerable<object> GetAll(Type service, string name = null, params IIocParameter[] parameters)
            {
                return _resolver.ResolveMany(service, serviceKey: name);
            }

            public void BindToConstant(Type service, object instance, string name = null)
            {
                _parentContainer.BindToConstant(service, instance, name);
            }

            public void BindToMethod(Type service,
                Func<IIocContainer, IList<IIocParameter>, object> methodBindingDelegate, DependencyLifecycle lifecycle,
                string name = null,
                params IIocParameter[] parameters)
            {
                _parentContainer.BindToMethod(service, methodBindingDelegate, lifecycle, name, parameters);
            }

            public void Bind(Type service, Type typeTo, DependencyLifecycle lifecycle, string name = null,
                params IIocParameter[] parameters)
            {
                _parentContainer.Bind(service, typeTo, lifecycle, name, parameters);
            }

            public void Unbind(Type service)
            {
                _parentContainer.Unbind(service);
            }

            public bool CanResolve(Type service, string name = null)
            {
                return _parentContainer.CanResolve(service, name);
            }

            public int Id { get; }

            public IIocContainer Parent => _parentContainer;

            public object Container => _resolver;

            public object GetService(Type serviceType)
            {
                return Get(serviceType);
            }
        }
    }
}