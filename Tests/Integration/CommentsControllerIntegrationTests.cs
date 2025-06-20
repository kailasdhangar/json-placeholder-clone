using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JsonPlaceholderClone.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JsonPlaceholderClone.Tests.Integration;

public class CommentsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CommentsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllComments_ReturnsOkAndComments()
    {
        var response = await _client.GetAsync("/api/comments");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        comments.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCommentById_ReturnsOkAndComment_WhenExists()
    {
        var response = await _client.GetAsync("/api/comments/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comment = await response.Content.ReadFromJsonAsync<CommentDto>();
        comment.Should().NotBeNull();
        comment!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetCommentById_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/comments/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateComment_ReturnsCreatedAndComment()
    {
        var createDto = new CreateCommentDto
        {
            PostId = 1,
            Name = "Integration Test Commenter",
            Email = "integration@example.com",
            Body = "This is a test comment."
        };
        var response = await _client.PostAsJsonAsync("/api/comments", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var comment = await response.Content.ReadFromJsonAsync<CommentDto>();
        comment.Should().NotBeNull();
        comment!.Name.Should().Be("Integration Test Commenter");
    }

    [Fact]
    public async Task CreateComment_ReturnsBadRequest_WhenInvalid()
    {
        var createDto = new CreateCommentDto { PostId = 0, Name = "", Email = "", Body = "" };
        var response = await _client.PostAsJsonAsync("/api/comments", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateComment_ReturnsOkAndUpdatedComment()
    {
        var updateDto = new UpdateCommentDto { Name = "Updated Name", Email = "updated@example.com", Body = "Updated Body" };
        var response = await _client.PutAsJsonAsync("/api/comments/1", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comment = await response.Content.ReadFromJsonAsync<CommentDto>();
        comment.Should().NotBeNull();
        comment!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateComment_ReturnsNotFound_WhenNotExists()
    {
        var updateDto = new UpdateCommentDto { Name = "Updated Name" };
        var response = await _client.PutAsJsonAsync("/api/comments/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNoContent_WhenExists()
    {
        var createDto = new CreateCommentDto { PostId = 1, Name = "To Delete", Email = "delete@example.com", Body = "Delete me" };
        var createResponse = await _client.PostAsJsonAsync("/api/comments", createDto);
        var comment = await createResponse.Content.ReadFromJsonAsync<CommentDto>();
        var response = await _client.DeleteAsync($"/api/comments/{comment!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/api/comments/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCommentsByPostId_ReturnsOkAndComments()
    {
        var response = await _client.GetAsync("/api/posts/1/comments");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        comments.Should().NotBeNull();
    }
} 