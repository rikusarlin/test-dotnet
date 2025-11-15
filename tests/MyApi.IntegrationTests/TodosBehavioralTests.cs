using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyApi.Models;
using Xunit;

namespace MyApi.IntegrationTests;

public class TodosBehaviorTests
{
    [Fact]
    public async Task ListTodos_ShouldReturnEmptyInitially()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given nothing in DB

        // When
        var response = await client.GetAsync("/todos");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTodo_WithValidTitle_ShouldSucceed()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var newTodo = new Todo { Title = "Learn C#" };

        // When
        var response = await client.PostAsJsonAsync("/todos", newTodo);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Todo>();
        created.Should().NotBeNull();
        created!.Title.Should().Be("Learn C#");
        created.Completed.Should().BeFalse();
        created.Id.Should().BePositive();
    }

    [Fact]
    public async Task CreateTodo_WithInvalidTitle_ShouldReturnBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var invalidTodo = new Todo { Title = "   " };

        // When
        var response = await client.PostAsJsonAsync("/todos", invalidTodo);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoById_ShouldReturnCorrectTodo()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var postResponse = await client.PostAsJsonAsync("/todos", new Todo { Title = "GetById" });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        // When
        var getResponse = await client.GetAsync($"/todos/{created!.Id}");

        // Then
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<Todo>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Title.Should().Be("GetById");
    }

    [Fact]
    public async Task GetTodoById_NonExisting_ShouldReturnNotFound()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var nonExistingId = 999;

        // When
        var response = await client.GetAsync($"/todos/{nonExistingId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ShouldSucceed()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var postResponse = await client.PostAsJsonAsync("/todos", new Todo { Title = "Initial", Completed = false });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        var update = new Todo { Title = "Updated", Completed = true };

        // When
        var putResponse = await client.PutAsJsonAsync($"/todos/{created!.Id}", update);

        // Then
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTodo = await putResponse.Content.ReadFromJsonAsync<Todo>();
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Title.Should().Be("Updated");
        updatedTodo.Completed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTodo_NonExisting_ShouldReturnNotFound()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var nonExistingId = 888;
        var update = new Todo { Title = "Update", Completed = true };

        // When
        var response = await client.PutAsJsonAsync($"/todos/{nonExistingId}", update);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTodo_WithInvalidTitle_ShouldReturnBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var postResponse = await client.PostAsJsonAsync("/todos", new Todo { Title = "Valid" });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        var invalidUpdate = new Todo { Title = "   " };

        // When
        var response = await client.PutAsJsonAsync($"/todos/{created!.Id}", invalidUpdate);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTodo_ShouldSucceed()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var postResponse = await client.PostAsJsonAsync("/todos", new Todo { Title = "ToDelete" });
        var created = await postResponse.Content.ReadFromJsonAsync<Todo>();

        // When
        var deleteResponse = await client.DeleteAsync($"/todos/{created!.Id}");

        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // And the todo should no longer exist
        var getResponse = await client.GetAsync($"/todos/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_NonExisting_ShouldReturnNotFound()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Given
        var nonExistingId = 777;

        // When
        var response = await client.DeleteAsync($"/todos/{nonExistingId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
