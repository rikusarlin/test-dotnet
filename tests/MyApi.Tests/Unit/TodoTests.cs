using MyApi.Models;
using Xunit;

namespace MyApi.Tests.Unit
{
    public class TodoTests
    {
        [Fact]
        public void Todo_DefaultCompleted_ShouldBeFalse()
        {
            var todo = new Todo();

            Assert.False(todo.Completed);
        }

        [Fact]
        public void Todo_Title_ShouldStoreAndReturnCorrectValue()
        {
            var todo = new Todo { Title = "Learn C#" };

            Assert.Equal("Learn C#", todo.Title);
        }

        [Fact]
        public void Todo_Id_ShouldStoreCorrectValue()
        {
            var todo = new Todo { Id = 42 };

            Assert.Equal(42, todo.Id);
        }
    }
}
