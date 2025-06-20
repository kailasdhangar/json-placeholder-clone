using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JsonPlaceholderClone.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JsonPlaceholderClone.Tests.Integration;

public class TodosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodosControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTodos_ReturnsOkAndTodos()
    {
        var response = await _client.GetAsync("/api/todos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoDto>>();
        todos.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTodoById_ReturnsOkAndTodo_WhenExists()
    {
        var response = await _client.GetAsync("/api/todos/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoDto>();
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetTodoById_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/todos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTodo_ReturnsCreatedAndTodo()
    {
        var createDto = new CreateTodoDto
        {
            UserId = 1,
            Title = "Integration Test Todo",
            Completed = false
        };
        var response = await _client.PostAsJsonAsync("/api/todos", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await response.Content.ReadFromJsonAsync<TodoDto>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Integration Test Todo");
    }

    [Fact]
    public async Task CreateTodo_ReturnsBadRequest_WhenInvalid()
    {
        var createDto = new CreateTodoDto { UserId = 0, Title = "", Completed = false };
        var response = await _client.PostAsJsonAsync("/api/todos", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTodo_ReturnsOkAndUpdatedTodo()
    {
        var updateDto = new UpdateTodoDto { Title = "Updated Title", Completed = true };
        var response = await _client.PutAsJsonAsync("/api/todos/1", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoDto>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Updated Title");
        todo.Completed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTodo_ReturnsNotFound_WhenNotExists()
    {
        var updateDto = new UpdateTodoDto { Title = "Updated Title" };
        var response = await _client.PutAsJsonAsync("/api/todos/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_ReturnsNoContent_WhenExists()
    {
        var createDto = new CreateTodoDto { UserId = 1, Title = "To Delete", Completed = false };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createDto);
        var todo = await createResponse.Content.ReadFromJsonAsync<TodoDto>();
        var response = await _client.DeleteAsync($"/api/todos/{todo!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/api/todos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTodosByUserId_ReturnsOkAndTodos()
    {
        var response = await _client.GetAsync("/api/users/1/todos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoDto>>();
        todos.Should().NotBeNull();
    }
} 