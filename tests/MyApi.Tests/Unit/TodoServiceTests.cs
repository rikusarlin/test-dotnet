using Xunit;
using MyApi.Services;

namespace MyApi.Tests.Unit
{
    public class TodoLogicTests
    {
        [Theory]
        [InlineData("Learn C#", true)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void IsValidTitle_WorksAsExpected(string? input, bool expected)
        {
            var ok = TodoService.IsValidTitle(input);
            Assert.Equal(expected, ok);
        }
    }
}
