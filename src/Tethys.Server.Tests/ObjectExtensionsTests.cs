using Shouldly;
using Xunit;

namespace Tethys.Server.Tests
{
    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ObjectExtensions_HasValue_ReturnsFalse(string source)
        {
            source.HasValue().ShouldBeFalse();
        }
        [Theory]
        [InlineData("data")]
        [InlineData("data ")]
        [InlineData(" data ")]
        [InlineData("data ")]
        public void ObjectExtensions_HasValue_ReturnsTrue(string source)
        {
            source.HasValue().ShouldBeTrue();
        }
    }
}