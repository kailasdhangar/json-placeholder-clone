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

public class PhotoServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PhotoService>> _loggerMock;

    public PhotoServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<PhotoService>>();
    }

    [Fact]
    public async Task GetAllPhotosAsync_ShouldReturnAllPhotos()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var photos = new List<Photo>
        {
            new() { Id = 1, AlbumId = 1, Title = "Photo 1", Url = "http://example.com/photo1.jpg", ThumbnailUrl = "http://example.com/thumb1.jpg" },
            new() { Id = 2, AlbumId = 1, Title = "Photo 2", Url = "http://example.com/photo2.jpg", ThumbnailUrl = "http://example.com/thumb2.jpg" }
        };

        context.Photos.AddRange(photos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllPhotosAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Photo 1");
        result.Should().Contain(p => p.Title == "Photo 2");
    }

    [Fact]
    public async Task GetPhotoByIdAsync_WithValidId_ShouldReturnPhoto()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var photo = new Photo { Id = 1, AlbumId = 1, Title = "Test Photo", Url = "http://example.com/test.jpg", ThumbnailUrl = "http://example.com/test-thumb.jpg" };
        context.Photos.Add(photo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPhotoByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Photo");
        result.Url.Should().Be("http://example.com/test.jpg");
        result.ThumbnailUrl.Should().Be("http://example.com/test-thumb.jpg");
    }

    [Fact]
    public async Task GetPhotoByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetPhotoByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePhotoAsync_WithValidData_ShouldCreatePhoto()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var album = new Album { Id = 1, UserId = 1, Title = "Test Album" };
        context.Albums.Add(album);
        await context.SaveChangesAsync();

        var createPhotoDto = new CreatePhotoDto
        {
            AlbumId = 1,
            Title = "New Photo",
            Url = "http://example.com/new-photo.jpg",
            ThumbnailUrl = "http://example.com/new-photo-thumb.jpg"
        };

        // Act
        var result = await service.CreatePhotoAsync(createPhotoDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Photo");
        result.Url.Should().Be("http://example.com/new-photo.jpg");
        result.ThumbnailUrl.Should().Be("http://example.com/new-photo-thumb.jpg");
        result.AlbumId.Should().Be(1);

        // Verify it was saved to database
        var savedPhoto = await context.Photos.FirstOrDefaultAsync(p => p.Title == "New Photo");
        savedPhoto.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePhotoAsync_WithInvalidAlbumId_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var createPhotoDto = new CreatePhotoDto
        {
            AlbumId = 999, // Non-existent album
            Title = "New Photo",
            Url = "http://example.com/new-photo.jpg",
            ThumbnailUrl = "http://example.com/new-photo-thumb.jpg"
        };

        // Act & Assert
        await service.Invoking(s => s.CreatePhotoAsync(createPhotoDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Album not found");
    }

    [Fact]
    public async Task UpdatePhotoAsync_WithValidData_ShouldUpdatePhoto()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var photo = new Photo { Id = 1, AlbumId = 1, Title = "Original Title", Url = "http://example.com/original.jpg", ThumbnailUrl = "http://example.com/original-thumb.jpg" };
        context.Photos.Add(photo);
        await context.SaveChangesAsync();

        var updatePhotoDto = new UpdatePhotoDto
        {
            Title = "Updated Title",
            Url = "http://example.com/updated.jpg",
            ThumbnailUrl = "http://example.com/updated-thumb.jpg"
        };

        // Act
        var result = await service.UpdatePhotoAsync(1, updatePhotoDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Url.Should().Be("http://example.com/updated.jpg");
        result.ThumbnailUrl.Should().Be("http://example.com/updated-thumb.jpg");
    }

    [Fact]
    public async Task UpdatePhotoAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var updatePhotoDto = new UpdatePhotoDto { Title = "Updated Title" };

        // Act
        var result = await service.UpdatePhotoAsync(999, updatePhotoDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeletePhotoAsync_WithValidId_ShouldDeletePhoto()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var photo = new Photo { Id = 1, AlbumId = 1, Title = "Test Photo", Url = "http://example.com/test.jpg", ThumbnailUrl = "http://example.com/test-thumb.jpg" };
        context.Photos.Add(photo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeletePhotoAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify photo was deleted
        var deletedPhoto = await context.Photos.FindAsync(1);
        deletedPhoto.Should().BeNull();
    }

    [Fact]
    public async Task DeletePhotoAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeletePhotoAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPhotosByAlbumIdAsync_ShouldReturnPhotosForAlbum()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PhotoService(context, _mapper, _loggerMock.Object);

        var album1 = new Album { Id = 1, UserId = 1, Title = "Album 1" };
        var album2 = new Album { Id = 2, UserId = 1, Title = "Album 2" };
        var photos = new List<Photo>
        {
            new() { Id = 1, AlbumId = 1, Title = "Photo 1", Url = "http://example.com/photo1.jpg", ThumbnailUrl = "http://example.com/thumb1.jpg" },
            new() { Id = 2, AlbumId = 1, Title = "Photo 2", Url = "http://example.com/photo2.jpg", ThumbnailUrl = "http://example.com/thumb2.jpg" },
            new() { Id = 3, AlbumId = 2, Title = "Photo 3", Url = "http://example.com/photo3.jpg", ThumbnailUrl = "http://example.com/thumb3.jpg" }
        };

        context.Albums.AddRange(album1, album2);
        context.Photos.AddRange(photos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPhotosByAlbumIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Photo 1");
        result.Should().Contain(p => p.Title == "Photo 2");
        result.Should().NotContain(p => p.Title == "Photo 3"); // Should not include photo from album 2
    }
} 