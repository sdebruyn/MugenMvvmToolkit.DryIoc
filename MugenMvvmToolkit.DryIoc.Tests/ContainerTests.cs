using MugenMvvmToolkit.Models.IoC;
using Xunit;

namespace MugenMvvmToolkit.DryIoc.Tests
{
    public class ContainerTests: IClassFixture<ContainerFixture>
    {
        readonly ContainerFixture _fixture;

        public ContainerTests(ContainerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void Container_BindGet_Instance()
        {
            // Arrange
            _fixture.Reset();
            var firstCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            _fixture.DryIocContainer.Bind<IMockable, Mockable>(DependencyLifecycle.TransientInstance);
            
            // Act
            var secondCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            var resolved = _fixture.DryIocContainer.Get<IMockable>();
            
            // Assert
            Assert.False(firstCanResolve);
            Assert.True(secondCanResolve);
            Assert.NotNull(resolved);
            Assert.IsType<Mockable>(resolved);
        }

        [Fact]
        public void Container_BindConstantGet_Instance()
        {
            // Arrange
            _fixture.Reset();
            var firstCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            var instance = new Mockable();
            _fixture.DryIocContainer.BindToConstant<IMockable>(instance);
            
            // Act
            var secondCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            var resolved = _fixture.DryIocContainer.Get<IMockable>();
            
            // Assert
            Assert.False(firstCanResolve);
            Assert.True(secondCanResolve);
            Assert.NotNull(resolved);
            Assert.IsType<Mockable>(resolved);
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void Container_BindMethodGet_Instance()
        {
            // Arrange
            _fixture.Reset();
            var firstCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            _fixture.DryIocContainer.BindToMethod<IMockable>((container, list) => new Mockable(), DependencyLifecycle.TransientInstance);
            
            // Act
            var secondCanResolve = _fixture.DryIocContainer.CanResolve<IMockable>();
            var resolved = _fixture.DryIocContainer.Get<IMockable>();
            
            // Assert
            Assert.False(firstCanResolve);
            Assert.True(secondCanResolve);
            Assert.NotNull(resolved);
            Assert.IsType<Mockable>(resolved);
        }
    }
}