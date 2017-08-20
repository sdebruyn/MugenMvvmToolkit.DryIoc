using System;
using System.Collections.Generic;
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
        public static readonly DependencyLifecycle SingleInstancePerThread;
        public static readonly DependencyLifecycle SingleInstancePerResolution;

        static int _idCounter;

        readonly IContainer _container;
        readonly DryIocContainer _parent;
        bool _disposed;

        static DryIocContainer()
        {
            SingleInstancePerThread = new DependencyLifecycle("Thread");
            SingleInstancePerResolution = new DependencyLifecycle("ResolutionScope");
        }

        DryIocContainer(Rules rules)
        {
            _container = new Container(rules);
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

        public bool IsDisposed => _container is Container container ? container.IsDisposed : _disposed;

        public event EventHandler<IDisposableObject, EventArgs> Disposed;

        public IIocContainer CreateChild()
        {
            return new DryIocContainer(this);
        }

        public object Get(Type service, string name = null, params IIocParameter[] parameters)
        {
            ThrowForUnusedParameters(parameters);
            return _container.Resolve(service, name);
        }

        internal static void ThrowForUnusedParameters(IIocParameter[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                throw new ArgumentException("The given parameters are not supported by this IoC container.", nameof(parameters));
            }
        }

        public IEnumerable<object> GetAll(Type service, string name = null, params IIocParameter[] parameters)
        {
            ThrowForUnusedParameters(parameters);
            return _container.ResolveMany(service, serviceKey: name);
        }

        public void BindToConstant(Type service, object instance, string name = null)
        {
            _container.UseInstance(service, instance, serviceKey: name);
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
            ThrowForUnusedParameters(parameters);
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

            if (lifecycle == SingleInstancePerThread)
                return Reuse.InThread;

            if (lifecycle == SingleInstancePerResolution)
                return Reuse.InResolutionScope;

            return null;
        }

        public sealed class Builder
        {
            readonly Rules _rules;

            public Builder()
            {
                _rules = Rules.Default;
            }

            Builder(Rules rules)
            {
                _rules = rules;
            }

            public Builder DisableDisposeWarnings()
            {
                return !_rules.ThrowOnRegisteringDisposableTransient
                    ? this
                    : new Builder(_rules.WithoutThrowOnRegisteringDisposableTransient());
            }

            public DryIocContainer Build()
            {
                return new DryIocContainer(_rules.With(FactoryMethod.ConstructorWithResolvableArguments)
                    .WithAutoConcreteTypeResolution());
            }
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
                ThrowForUnusedParameters(parameters);
                return _resolver.Resolve(service, name);
            }

            public IEnumerable<object> GetAll(Type service, string name = null, params IIocParameter[] parameters)
            {
                ThrowForUnusedParameters(parameters);
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
                ThrowForUnusedParameters(parameters);
                _parentContainer.BindToMethod(service, methodBindingDelegate, lifecycle, name, parameters);
            }

            public void Bind(Type service, Type typeTo, DependencyLifecycle lifecycle, string name = null,
                params IIocParameter[] parameters)
            {
                ThrowForUnusedParameters(parameters);
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