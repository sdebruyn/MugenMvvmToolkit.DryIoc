using Xunit;
using DryIoc;

namespace MugenMvvmToolkit.DryIoc.Tests
{
    public class ContainerBuilderTests
    {
        [Fact]
        public void Builder_DefaultOptions_NewContainer()
        {
            // Act
            var container = new DryIocContainer.Builder().Build();

            // Assert
            Assert.NotNull(container);
            Assert.IsType<DryIocContainer>(container);
            Assert.NotNull(container.Container);
            Assert.IsType<Container>(container.Container);
        }

        [Fact]
        public void Builder_NoDisposeExcOptions_NewContainer()
        {
            // Act
            var container = new DryIocContainer.Builder().DisableDisposeWarnings().Build();

            // Assert
            Assert.NotNull(container);
            Assert.IsType<DryIocContainer>(container);
            Assert.NotNull(container.Container);
            Assert.IsType<Container>(container.Container);

            // Arrange
            var containerRules = ((Container)container.Container).Rules;
            Assert.False(containerRules.ThrowOnRegisteringDisposableTransient);
        }
    }
}
