using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JsonPlaceholderClone.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JsonPlaceholderClone.Tests.Integration;

public class AlbumsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AlbumsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAlbums_ReturnsOkAndAlbums()
    {
        var response = await _client.GetAsync("/api/albums");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var albums = await response.Content.ReadFromJsonAsync<List<AlbumDto>>();
        albums.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAlbumById_ReturnsOkAndAlbum_WhenExists()
    {
        var response = await _client.GetAsync("/api/albums/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var album = await response.Content.ReadFromJsonAsync<AlbumDto>();
        album.Should().NotBeNull();
        album!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetAlbumById_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/albums/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAlbum_ReturnsCreatedAndAlbum()
    {
        var createDto = new CreateAlbumDto
        {
            UserId = 1,
            Title = "Integration Test Album"
        };
        var response = await _client.PostAsJsonAsync("/api/albums", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var album = await response.Content.ReadFromJsonAsync<AlbumDto>();
        album.Should().NotBeNull();
        album!.Title.Should().Be("Integration Test Album");
    }

    [Fact]
    public async Task CreateAlbum_ReturnsBadRequest_WhenInvalid()
    {
        var createDto = new CreateAlbumDto { UserId = 0, Title = "" };
        var response = await _client.PostAsJsonAsync("/api/albums", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAlbum_ReturnsOkAndUpdatedAlbum()
    {
        var updateDto = new UpdateAlbumDto { Title = "Updated Title" };
        var response = await _client.PutAsJsonAsync("/api/albums/1", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var album = await response.Content.ReadFromJsonAsync<AlbumDto>();
        album.Should().NotBeNull();
        album!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAlbum_ReturnsNotFound_WhenNotExists()
    {
        var updateDto = new UpdateAlbumDto { Title = "Updated Title" };
        var response = await _client.PutAsJsonAsync("/api/albums/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAlbum_ReturnsNoContent_WhenExists()
    {
        var createDto = new CreateAlbumDto { UserId = 1, Title = "To Delete" };
        var createResponse = await _client.PostAsJsonAsync("/api/albums", createDto);
        var album = await createResponse.Content.ReadFromJsonAsync<AlbumDto>();
        var response = await _client.DeleteAsync($"/api/albums/{album!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAlbum_ReturnsNotFound_WhenNotExists()
    {
        var response = await _client.DeleteAsync("/api/albums/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAlbumPhotos_ReturnsOkAndPhotos()
    {
        var response = await _client.GetAsync("/api/albums/1/photos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photos = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        photos.Should().NotBeNull();
    }
} 