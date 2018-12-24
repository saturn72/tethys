using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Tethys.Server.Tests
{
    public class ObjectExtensionsTests
    {
        #region HasValue

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

        #endregion
        #region IsNullOrEmpty

        public static IEnumerable<object[]> EmptyCollections =>
        new[]{
            new object[]{null},
            new object[]{new string[]{}}
        };
        [Theory]
        [MemberData(nameof(EmptyCollections))]
        public void ObjectExtensions_IsNullOrEmpty_ReturnsTrue(IEnumerable<object> collection)
        {
            collection.IsNullOrEmpty().ShouldBeTrue();
        }


        [Fact]
        public void ObjectExtensions_IsNullOrEmpty_ReturnsFalse()
        {
            new[] { "a", "b", "c" }.IsNullOrEmpty().ShouldBeFalse();
        }

        #endregion
    }
}