using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using JsonPlaceholderClone.Services;
using JsonPlaceholderClone.Mappings;

namespace JsonPlaceholderClone.Tests.Unit;

public class UserServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _loggerMock;

    public UserServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<UserService>>();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var users = new List<User>
        {
            new() { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" },
            new() { Id = 2, Name = "Jane Smith", Username = "janesmith", Email = "jane@example.com" }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Name == "John Doe");
        result.Should().Contain(u => u.Name == "Jane Smith");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Doe");
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var createUserDto = new CreateUserDto
        {
            Name = "John Doe",
            Username = "johndoe",
            Email = "john@example.com",
            Address = new AddressDto
            {
                Street = "123 Main St",
                Suite = "Apt 1",
                City = "New York",
                Zipcode = "10001",
                Geo = new GeoDto { Lat = "40.7128", Lng = "-74.0060" }
            },
            Company = new CompanyDto
            {
                Name = "Tech Corp",
                CatchPhrase = "Innovation at its best",
                Bs = "harness real-time e-markets"
            }
        };

        // Act
        var result = await service.CreateUserAsync(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
        result.Address.Street.Should().Be("123 Main St");
        result.Company.Name.Should().Be("Tech Corp");

        // Verify it was saved to database
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "john@example.com");
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var existingUser = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var createUserDto = new CreateUserDto
        {
            Name = "Jane Smith",
            Username = "janesmith",
            Email = "john@example.com", // Duplicate email
            Address = new AddressDto(),
            Company = new CompanyDto()
        };

        // Act & Assert
        await service.Invoking(s => s.CreateUserAsync(createUserDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User with this email or username already exists");
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var updateUserDto = new UpdateUserDto
        {
            Name = "John Updated",
            Email = "john.updated@example.com"
        };

        // Act
        var result = await service.UpdateUserAsync(1, updateUserDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Updated");
        result.Email.Should().Be("john.updated@example.com");
        result.Username.Should().Be("johndoe"); // Should remain unchanged
    }

    [Fact]
    public async Task UpdateUserAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var updateUserDto = new UpdateUserDto { Name = "John Updated" };

        // Act
        var result = await service.UpdateUserAsync(999, updateUserDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ShouldDeleteUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteUserAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify user was deleted
        var deletedUser = await context.Users.FindAsync(1);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteUserAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserPostsAsync_ShouldReturnUserPosts()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        var posts = new List<Post>
        {
            new() { Id = 1, UserId = 1, Title = "Post 1", Body = "Body 1" },
            new() { Id = 2, UserId = 1, Title = "Post 2", Body = "Body 2" }
        };

        context.Users.Add(user);
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserPostsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Post 1");
        result.Should().Contain(p => p.Title == "Post 2");
    }

    [Fact]
    public async Task GetUserAlbumsAsync_ShouldReturnUserAlbums()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        var albums = new List<Album>
        {
            new() { Id = 1, UserId = 1, Title = "Album 1" },
            new() { Id = 2, UserId = 1, Title = "Album 2" }
        };

        context.Users.Add(user);
        context.Albums.AddRange(albums);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserAlbumsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.Title == "Album 1");
        result.Should().Contain(a => a.Title == "Album 2");
    }

    [Fact]
    public async Task GetUserTodosAsync_ShouldReturnUserTodos()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new UserService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        var todos = new List<Todo>
        {
            new() { Id = 1, UserId = 1, Title = "Todo 1", Completed = false },
            new() { Id = 2, UserId = 1, Title = "Todo 2", Completed = true }
        };

        context.Users.Add(user);
        context.Todos.AddRange(todos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserTodosAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Todo 1" && !t.Completed);
        result.Should().Contain(t => t.Title == "Todo 2" && t.Completed);
    }
} 