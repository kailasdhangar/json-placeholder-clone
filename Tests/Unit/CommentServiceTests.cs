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

public class CommentServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CommentService>> _loggerMock;

    public CommentServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<CommentService>>();
    }

    [Fact]
    public async Task GetAllCommentsAsync_ShouldReturnAllComments()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var comments = new List<Comment>
        {
            new() { Id = 1, PostId = 1, Name = "Commenter 1", Email = "comment1@example.com", Body = "Comment 1" },
            new() { Id = 2, PostId = 1, Name = "Commenter 2", Email = "comment2@example.com", Body = "Comment 2" }
        };

        context.Comments.AddRange(comments);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllCommentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Commenter 1");
        result.Should().Contain(c => c.Name == "Commenter 2");
    }

    [Fact]
    public async Task GetCommentByIdAsync_WithValidId_ShouldReturnComment()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var comment = new Comment { Id = 1, PostId = 1, Name = "Test Commenter", Email = "test@example.com", Body = "Test Comment" };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetCommentByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Commenter");
        result.Email.Should().Be("test@example.com");
        result.Body.Should().Be("Test Comment");
    }

    [Fact]
    public async Task GetCommentByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetCommentByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCommentAsync_WithValidData_ShouldCreateComment()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var post = new Post { Id = 1, UserId = 1, Title = "Test Post", Body = "Test Body" };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var createCommentDto = new CreateCommentDto
        {
            PostId = 1,
            Name = "New Commenter",
            Email = "newcommenter@example.com",
            Body = "New Comment Body"
        };

        // Act
        var result = await service.CreateCommentAsync(createCommentDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Commenter");
        result.Email.Should().Be("newcommenter@example.com");
        result.Body.Should().Be("New Comment Body");
        result.PostId.Should().Be(1);

        // Verify it was saved to database
        var savedComment = await context.Comments.FirstOrDefaultAsync(c => c.Name == "New Commenter");
        savedComment.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateCommentAsync_WithInvalidPostId_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var createCommentDto = new CreateCommentDto
        {
            PostId = 999, // Non-existent post
            Name = "New Commenter",
            Email = "newcommenter@example.com",
            Body = "New Comment Body"
        };

        // Act & Assert
        await service.Invoking(s => s.CreateCommentAsync(createCommentDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Post not found");
    }

    [Fact]
    public async Task UpdateCommentAsync_WithValidData_ShouldUpdateComment()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var comment = new Comment { Id = 1, PostId = 1, Name = "Original Name", Email = "original@example.com", Body = "Original Body" };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        var updateCommentDto = new UpdateCommentDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Body = "Updated Body"
        };

        // Act
        var result = await service.UpdateCommentAsync(1, updateCommentDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@example.com");
        result.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task UpdateCommentAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var updateCommentDto = new UpdateCommentDto { Name = "Updated Name" };

        // Act
        var result = await service.UpdateCommentAsync(999, updateCommentDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCommentAsync_WithValidId_ShouldDeleteComment()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var comment = new Comment { Id = 1, PostId = 1, Name = "Test Commenter", Email = "test@example.com", Body = "Test Comment" };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteCommentAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify comment was deleted
        var deletedComment = await context.Comments.FindAsync(1);
        deletedComment.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCommentAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteCommentAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetCommentsByPostIdAsync_ShouldReturnCommentsForPost()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new CommentService(context, _mapper, _loggerMock.Object);

        var post1 = new Post { Id = 1, UserId = 1, Title = "Post 1", Body = "Body 1" };
        var post2 = new Post { Id = 2, UserId = 1, Title = "Post 2", Body = "Body 2" };
        var comments = new List<Comment>
        {
            new() { Id = 1, PostId = 1, Name = "Commenter 1", Email = "comment1@example.com", Body = "Comment for Post 1" },
            new() { Id = 2, PostId = 1, Name = "Commenter 2", Email = "comment2@example.com", Body = "Another comment for Post 1" },
            new() { Id = 3, PostId = 2, Name = "Commenter 3", Email = "comment3@example.com", Body = "Comment for Post 2" }
        };

        context.Posts.AddRange(post1, post2);
        context.Comments.AddRange(comments);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetCommentsByPostIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Commenter 1");
        result.Should().Contain(c => c.Name == "Commenter 2");
        result.Should().NotContain(c => c.Name == "Commenter 3"); // Should not include comment from post 2
    }
} 