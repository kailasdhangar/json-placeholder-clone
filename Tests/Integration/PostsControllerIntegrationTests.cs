using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JsonPlaceholderClone.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JsonPlaceholderClone.Tests.Integration;

public class PostsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PostsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllPosts_ReturnsOkAndPosts()
    {
        var response = await _client.GetAsync("/api/posts");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        posts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPostById_ReturnsOkAndPost_WhenExists()
    {
        var response = await _client.GetAsync("/api/posts/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        post.Should().NotBeNull();
        post!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetPostById_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/posts/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreatedAndPost()
    {
        var createDto = new CreatePostDto
        {
            UserId = 1,
            Title = "Integration Test Post",
            Body = "This is a test post."
        };
        var response = await _client.PostAsJsonAsync("/api/posts", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        post.Should().NotBeNull();
        post!.Title.Should().Be("Integration Test Post");
    }

    [Fact]
    public async Task CreatePost_ReturnsBadRequest_WhenInvalid()
    {
        var createDto = new CreatePostDto { UserId = 0, Title = "", Body = "" };
        var response = await _client.PostAsJsonAsync("/api/posts", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePost_ReturnsOkAndUpdatedPost()
    {
        var updateDto = new UpdatePostDto { Title = "Updated Title", Body = "Updated Body" };
        var response = await _client.PutAsJsonAsync("/api/posts/1", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        post.Should().NotBeNull();
        post!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdatePost_ReturnsNotFound_WhenNotExists()
    {
        var updateDto = new UpdatePostDto { Title = "Updated Title" };
        var response = await _client.PutAsJsonAsync("/api/posts/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePost_ReturnsNoContent_WhenExists()
    {
        var createDto = new CreatePostDto { UserId = 1, Title = "To Delete", Body = "Delete me" };
        var createResponse = await _client.PostAsJsonAsync("/api/posts", createDto);
        var post = await createResponse.Content.ReadFromJsonAsync<PostDto>();
        var response = await _client.DeleteAsync($"/api/posts/{post!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePost_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/api/posts/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPostComments_ReturnsOkAndComments()
    {
        var response = await _client.GetAsync("/api/posts/1/comments");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        comments.Should().NotBeNull();
    }
} 