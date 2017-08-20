using System;

namespace MugenMvvmToolkit.DryIoc.Tests
{
    public class ContainerFixture : IDisposable
    {
        public DryIocContainer DryIocContainer { get; private set; }

        public ContainerFixture()
        {
            Reset();
        }

        public void Reset()
        {
            DryIocContainer?.Dispose();
            DryIocContainer = new DryIocContainer.Builder().Build();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DryIocContainer?.Dispose();
                DryIocContainer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}