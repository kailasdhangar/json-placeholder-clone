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

public class TodoServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TodoService>> _loggerMock;

    public TodoServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _loggerMock = new Mock<ILogger<TodoService>>();
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnAllTodos()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var todos = new List<Todo>
        {
            new() { Id = 1, UserId = 1, Title = "Todo 1", Completed = false },
            new() { Id = 2, UserId = 1, Title = "Todo 2", Completed = true }
        };

        context.Todos.AddRange(todos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllTodosAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Todo 1");
        result.Should().Contain(t => t.Title == "Todo 2");
    }

    [Fact]
    public async Task GetTodoByIdAsync_WithValidId_ShouldReturnTodo()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var todo = new Todo { Id = 1, UserId = 1, Title = "Test Todo", Completed = false };
        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetTodoByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Todo");
        result.Completed.Should().BeFalse();
    }

    [Fact]
    public async Task GetTodoByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetTodoByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateTodoAsync_WithValidData_ShouldCreateTodo()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var user = new User { Id = 1, Name = "John Doe", Username = "johndoe", Email = "john@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var createTodoDto = new CreateTodoDto
        {
            UserId = 1,
            Title = "New Todo",
            Completed = false
        };

        // Act
        var result = await service.CreateTodoAsync(createTodoDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Todo");
        result.Completed.Should().BeFalse();
        result.UserId.Should().Be(1);

        // Verify it was saved to database
        var savedTodo = await context.Todos.FirstOrDefaultAsync(t => t.Title == "New Todo");
        savedTodo.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTodoAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var createTodoDto = new CreateTodoDto
        {
            UserId = 999, // Non-existent user
            Title = "New Todo",
            Completed = false
        };

        // Act & Assert
        await service.Invoking(s => s.CreateTodoAsync(createTodoDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task UpdateTodoAsync_WithValidData_ShouldUpdateTodo()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var todo = new Todo { Id = 1, UserId = 1, Title = "Original Title", Completed = false };
        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        var updateTodoDto = new UpdateTodoDto
        {
            Title = "Updated Title",
            Completed = true
        };

        // Act
        var result = await service.UpdateTodoAsync(1, updateTodoDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Completed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTodoAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var updateTodoDto = new UpdateTodoDto { Title = "Updated Title" };

        // Act
        var result = await service.UpdateTodoAsync(999, updateTodoDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTodoAsync_WithValidId_ShouldDeleteTodo()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var todo = new Todo { Id = 1, UserId = 1, Title = "Test Todo", Completed = false };
        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteTodoAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify todo was deleted
        var deletedTodo = await context.Todos.FindAsync(1);
        deletedTodo.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTodoAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteTodoAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTodosByUserIdAsync_ShouldReturnTodosForUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new TodoService(context, _mapper, _loggerMock.Object);

        var user1 = new User { Id = 1, Name = "User 1", Username = "user1", Email = "user1@example.com" };
        var user2 = new User { Id = 2, Name = "User 2", Username = "user2", Email = "user2@example.com" };
        var todos = new List<Todo>
        {
            new() { Id = 1, UserId = 1, Title = "Todo 1", Completed = false },
            new() { Id = 2, UserId = 1, Title = "Todo 2", Completed = true },
            new() { Id = 3, UserId = 2, Title = "Todo 3", Completed = false }
        };

        context.Users.AddRange(user1, user2);
        context.Todos.AddRange(todos);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetTodosByUserIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Todo 1");
        result.Should().Contain(t => t.Title == "Todo 2");
        result.Should().NotContain(t => t.Title == "Todo 3"); // Should not include todo from user 2
    }
} 