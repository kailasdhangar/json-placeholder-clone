using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Tests.Integration;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserDto>>(content, _jsonOptions);
        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUser_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserDto>(content, _jsonOptions);
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createUserDto = new CreateUserDto
        {
            Name = "Test User",
            Username = "testuser",
            Email = "test@example.com",
            Address = new AddressDto
            {
                Street = "123 Test St",
                Suite = "Apt 1",
                City = "Test City",
                Zipcode = "12345",
                Geo = new GeoDto { Lat = "40.7128", Lng = "-74.0060" }
            },
            Company = new CompanyDto
            {
                Name = "Test Company",
                CatchPhrase = "Test catch phrase",
                Bs = "test bs"
            }
        };

        var json = JsonSerializer.Serialize(createUserDto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdUser = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions);
        createdUser.Should().NotBeNull();
        createdUser!.Name.Should().Be("Test User");
        createdUser.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task CreateUser_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createUserDto = new CreateUserDto
        {
            Name = "", // Invalid: empty name
            Username = "testuser",
            Email = "invalid-email", // Invalid email
            Address = new AddressDto(),
            Company = new CompanyDto()
        };

        var json = JsonSerializer.Serialize(createUserDto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        var updateUserDto = new UpdateUserDto
        {
            Name = "Updated User",
            Email = "updated@example.com"
        };

        var json = JsonSerializer.Serialize(updateUserDto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/users/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedUser = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("Updated User");
        updatedUser.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task UpdateUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var updateUserDto = new UpdateUserDto { Name = "Updated User" };

        var json = JsonSerializer.Serialize(updateUserDto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/users/999", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldDeleteUser()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserPosts_ShouldReturnUserPosts()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/1/posts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var posts = JsonSerializer.Deserialize<List<PostDto>>(content, _jsonOptions);
        posts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserAlbums_ShouldReturnUserAlbums()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/1/albums");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var albums = JsonSerializer.Deserialize<List<AlbumDto>>(content, _jsonOptions);
        albums.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserTodos_ShouldReturnUserTodos()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/1/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<TodoDto>>(content, _jsonOptions);
        todos.Should().NotBeNull();
    }
} 