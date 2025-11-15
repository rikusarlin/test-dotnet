using System.Net;
using System.Net.Http.Json;
using MyApi.Models;
using Xunit;

namespace MyApi.IntegrationTests;

public class TodosEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TodosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTodo_ShouldCreateTodo()
    {
        var newTodo = new Todo { Title = "Learn C#" };
        var response = await _client.PostAsJsonAsync("/todos", newTodo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(created);
        Assert.Equal("Learn C#", created!.Title);
        Assert.False(created.Completed);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task GetTodos_ShouldReturnList()
    {
        // Insert one todo
        await _client.PostAsJsonAsync("/todos", new Todo { Title = "Test GET list" });

        var todos = await _client.GetFromJsonAsync<List<Todo>>("/todos");
        Assert.NotNull(todos);
        Assert.Single(todos);
        Assert.Equal("Test GET list", todos[0].Title);
    }

    [Fact]
    public async Task GetTodoById_ShouldReturnCorrectTodo()
    {
        var response = await _client.PostAsJsonAsync("/todos", new Todo { Title = "GetById" });
        var created = await response.Content.ReadFromJsonAsync<Todo>();

        var getResponse = await _client.GetAsync($"/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<Todo>();
        Assert.Equal("GetById", fetched!.Title);
        Assert.Equal(created.Id, fetched.Id);
    }

    [Fact]
    public async Task PutTodo_ShouldUpdateTodo()
    {
        var postResponse = await _client.PostAsJsonAsync("/todos", new Todo { Title = "Initial", Completed = false });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        var updated = new Todo { Title = "Updated", Completed = true };
        var putResponse = await _client.PutAsJsonAsync($"/todos/{created!.Id}", updated);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        var updatedTodo = await putResponse.Content.ReadFromJsonAsync<Todo>();
        Assert.Equal("Updated", updatedTodo!.Title);
        Assert.True(updatedTodo.Completed);
    }

    [Fact]
    public async Task DeleteTodo_ShouldRemoveTodo()
    {
        var postResponse = await _client.PostAsJsonAsync("/todos", new Todo { Title = "ToDelete" });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        var deleteResponse = await _client.DeleteAsync($"/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task PostTodo_InvalidTitle_ShouldReturnBadRequest()
    {
        var invalid = new Todo { Title = "   " };
        var response = await _client.PostAsJsonAsync("/todos", invalid);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
