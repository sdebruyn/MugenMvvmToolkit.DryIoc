using System;
using Xunit;

namespace MugenMvvmToolkit.DryIoc.Tests
{
    public class DryIocContainerTests
    {
        [Fact]
        public void Builder_DefaultOptions_NewContainer()
        {
            // Arrange
            
            
            // Act
            var builder = new DryIocContainer.Builder();
            var container = builder.Build();
            
            // Assert
            Assert.NotNull(container);
            Assert.IsType<DryIocContainer>(container);
        }
    }
}
