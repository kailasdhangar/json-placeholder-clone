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

public class AlbumServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<AlbumService>> _loggerMock;

    public AlbumServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<AlbumService>>();
    }

    [Fact]
    public async Task GetAllAlbumsAsync_ShouldReturnAllAlbums()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var albums = new List<Album>
        {
            new() { Id = 1, UserId = 1, Title = "Album 1" },
            new() { Id = 2, UserId = 1, Title = "Album 2" }
        };

        context.Albums.AddRange(albums);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllAlbumsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.Title == "Album 1");
        result.Should().Contain(a => a.Title == "Album 2");
    }

    [Fact]
    public async Task GetAlbumByIdAsync_WithValidId_ShouldReturnAlbum()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Test Album" };
        context.Albums.Add(album);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAlbumByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Album");
        result.UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAlbumByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAlbumAsync_WithValidData_ShouldCreateAlbum()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var createAlbumDto = new CreateAlbumDto
        {
            UserId = 1,
            Title = "New Album"
        };

        // Act
        var result = await service.CreateAlbumAsync(createAlbumDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Album");
        result.UserId.Should().Be(1);

        // Verify it was saved to database
        var savedAlbum = await context.Albums.FirstOrDefaultAsync(a => a.Title == "New Album");
        savedAlbum.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAlbumAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var createAlbumDto = new CreateAlbumDto
        {
            UserId = 999, // Non-existent user
            Title = "New Album"
        };

        // Act & Assert
        await service.Invoking(s => s.CreateAlbumAsync(createAlbumDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task UpdateAlbumAsync_WithValidData_ShouldUpdateAlbum()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Original Title" };
        context.Albums.Add(album);
        await context.SaveChangesAsync();

        var updateAlbumDto = new UpdateAlbumDto
        {
            Title = "Updated Title"
        };

        // Act
        var result = await service.UpdateAlbumAsync(1, updateAlbumDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAlbumAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var updateAlbumDto = new UpdateAlbumDto { Title = "Updated Title" };

        // Act
        var result = await service.UpdateAlbumAsync(999, updateAlbumDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAlbumAsync_WithValidId_ShouldDeleteAlbum()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Test Album" };
        context.Albums.Add(album);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteAlbumAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify album was deleted
        var deletedAlbum = await context.Albums.FindAsync(1);
        deletedAlbum.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAlbumAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteAlbumAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAlbumPhotosAsync_ShouldReturnAlbumPhotos()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Test Album" };
        var photos = new List<Photo>
        {
            new() { Id = 1, AlbumId = 1, Title = "Photo 1", Url = "http://example.com/photo1.jpg", ThumbnailUrl = "http://example.com/thumb1.jpg" },
            new() { Id = 2, AlbumId = 1, Title = "Photo 2", Url = "http://example.com/photo2.jpg", ThumbnailUrl = "http://example.com/thumb2.jpg" }
        };

        context.Albums.Add(album);
        context.Photos.AddRange(photos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAlbumPhotosAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Photo 1");
        result.Should().Contain(p => p.Title == "Photo 2");
    }

    [Fact]
    public async Task GetAlbumWithPhotosAsync_ShouldReturnAlbumWithPhotos()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Test Album" };
        var photos = new List<Photo>
        {
            new() { Id = 1, AlbumId = 1, Title = "Photo 1", Url = "http://example.com/photo1.jpg", ThumbnailUrl = "http://example.com/thumb1.jpg" }
        };

        context.Albums.Add(album);
        context.Photos.AddRange(photos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAlbumWithPhotosAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Album");
        result.Photos.Should().HaveCount(1);
        result.Photos.Should().Contain(p => p.Title == "Photo 1");
    }

    [Fact]
    public async Task GetAlbumWithPhotosAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new AlbumService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAlbumWithPhotosAsync(999);

        // Assert
        result.Should().BeNull();
    }
} 