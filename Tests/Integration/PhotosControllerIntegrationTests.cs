using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JsonPlaceholderClone.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JsonPlaceholderClone.Tests.Integration;

public class PhotosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PhotosControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllPhotos_ReturnsOkAndPhotos()
    {
        var response = await _client.GetAsync("/api/photos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photos = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        photos.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPhotoById_ReturnsOkAndPhoto_WhenExists()
    {
        var response = await _client.GetAsync("/api/photos/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photo = await response.Content.ReadFromJsonAsync<PhotoDto>();
        photo.Should().NotBeNull();
        photo!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetPhotoById_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/photos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePhoto_ReturnsCreatedAndPhoto()
    {
        var createDto = new CreatePhotoDto
        {
            AlbumId = 1,
            Title = "Integration Test Photo",
            Url = "http://example.com/photo.jpg",
            ThumbnailUrl = "http://example.com/thumb.jpg"
        };
        var response = await _client.PostAsJsonAsync("/api/photos", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var photo = await response.Content.ReadFromJsonAsync<PhotoDto>();
        photo.Should().NotBeNull();
        photo!.Title.Should().Be("Integration Test Photo");
    }

    [Fact]
    public async Task CreatePhoto_ReturnsBadRequest_WhenInvalid()
    {
        var createDto = new CreatePhotoDto { AlbumId = 0, Title = "", Url = "", ThumbnailUrl = "" };
        var response = await _client.PostAsJsonAsync("/api/photos", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePhoto_ReturnsOkAndUpdatedPhoto()
    {
        var updateDto = new UpdatePhotoDto { Title = "Updated Title", Url = "http://example.com/updated.jpg", ThumbnailUrl = "http://example.com/updated-thumb.jpg" };
        var response = await _client.PutAsJsonAsync("/api/photos/1", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photo = await response.Content.ReadFromJsonAsync<PhotoDto>();
        photo.Should().NotBeNull();
        photo!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdatePhoto_ReturnsNotFound_WhenNotExists()
    {
        var updateDto = new UpdatePhotoDto { Title = "Updated Title" };
        var response = await _client.PutAsJsonAsync("/api/photos/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePhoto_ReturnsNoContent_WhenExists()
    {
        var createDto = new CreatePhotoDto { AlbumId = 1, Title = "To Delete", Url = "http://example.com/delete.jpg", ThumbnailUrl = "http://example.com/delete-thumb.jpg" };
        var createResponse = await _client.PostAsJsonAsync("/api/photos", createDto);
        var photo = await createResponse.Content.ReadFromJsonAsync<PhotoDto>();
        var response = await _client.DeleteAsync($"/api/photos/{photo!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePhoto_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/api/photos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPhotosByAlbumId_ReturnsOkAndPhotos()
    {
        var response = await _client.GetAsync("/api/albums/1/photos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photos = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        photos.Should().NotBeNull();
    }
} 