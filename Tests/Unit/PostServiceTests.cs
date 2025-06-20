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

public class PostServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PostService>> _loggerMock;

    public PostServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<PostService>>();
    }

    [Fact]
    public async Task GetAllPostsAsync_ShouldReturnAllPosts()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var posts = new List<Post>
        {
            new() { Id = 1, UserId = 1, Title = "Post 1", Body = "Body 1" },
            new() { Id = 2, UserId = 1, Title = "Post 2", Body = "Body 2" }
        };

        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllPostsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Post 1");
        result.Should().Contain(p => p.Title == "Post 2");
    }

    [Fact]
    public async Task GetPostByIdAsync_WithValidId_ShouldReturnPost()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Test Post", Body = "Test Body" };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPostByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Post");
        result.Body.Should().Be("Test Body");
    }

    [Fact]
    public async Task GetPostByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetPostByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePostAsync_WithValidData_ShouldCreatePost()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var createPostDto = new CreatePostDto
        {
            UserId = 1,
            Title = "New Post",
            Body = "New Post Body"
        };

        // Act
        var result = await service.CreatePostAsync(createPostDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Post");
        result.Body.Should().Be("New Post Body");
        result.UserId.Should().Be(1);

        // Verify it was saved to database
        var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Title == "New Post");
        savedPost.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePostAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var createPostDto = new CreatePostDto
        {
            UserId = 999, // Non-existent user
            Title = "New Post",
            Body = "New Post Body"
        };

        // Act & Assert
        await service.Invoking(s => s.CreatePostAsync(createPostDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task UpdatePostAsync_WithValidData_ShouldUpdatePost()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Original Title", Body = "Original Body" };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var updatePostDto = new UpdatePostDto
        {
            Title = "Updated Title",
            Body = "Updated Body"
        };

        // Act
        var result = await service.UpdatePostAsync(1, updatePostDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task UpdatePostAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var updatePostDto = new UpdatePostDto { Title = "Updated Title" };

        // Act
        var result = await service.UpdatePostAsync(999, updatePostDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeletePostAsync_WithValidId_ShouldDeletePost()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Test Post", Body = "Test Body" };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeletePostAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify post was deleted
        var deletedPost = await context.Posts.FindAsync(1);
        deletedPost.Should().BeNull();
    }

    [Fact]
    public async Task DeletePostAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeletePostAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPostCommentsAsync_ShouldReturnPostComments()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Test Post", Body = "Test Body" };
        var comments = new List<Comment>
        {
            new() { Id = 1, PostId = 1, Name = "Commenter 1", Email = "comment1@example.com", Body = "Comment 1" },
            new() { Id = 2, PostId = 1, Name = "Commenter 2", Email = "comment2@example.com", Body = "Comment 2" }
        };

        context.Posts.Add(post);
        context.Comments.AddRange(comments);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPostCommentsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Commenter 1");
        result.Should().Contain(c => c.Name == "Commenter 2");
    }

    [Fact]
    public async Task GetPostWithCommentsAsync_ShouldReturnPostWithComments()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Test Post", Body = "Test Body" };
        var comments = new List<Comment>
        {
            new() { Id = 1, PostId = 1, Name = "Commenter 1", Email = "comment1@example.com", Body = "Comment 1" }
        };

        context.Posts.Add(post);
        context.Comments.AddRange(comments);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPostWithCommentsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Post");
        result.Comments.Should().HaveCount(1);
        result.Comments.Should().Contain(c => c.Name == "Commenter 1");
    }

    [Fact]
    public async Task GetPostWithCommentsAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new PostService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetPostWithCommentsAsync(999);

        // Assert
        result.Should().BeNull();
    }
} 