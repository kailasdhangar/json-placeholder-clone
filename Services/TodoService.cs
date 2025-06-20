using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class TodoService : ITodoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ApplicationDbContext context, IMapper mapper, ILogger<TodoService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
    {
        try
        {
            var todos = await _context.Todos.ToListAsync();
            return _mapper.Map<IEnumerable<TodoDto>>(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all todos");
            throw;
        }
    }

    public async Task<TodoDto?> GetTodoByIdAsync(int id)
    {
        try
        {
            var todo = await _context.Todos.FindAsync(id);
            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todo with id {TodoId}", id);
            throw;
        }
    }

    public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
    {
        try
        {
            // Verify user exists
            var user = await _context.Users.FindAsync(createTodoDto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var todo = _mapper.Map<Todo>(createTodoDto);
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new todo with id {TodoId}", todo.Id);
            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating todo");
            throw;
        }
    }

    public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
    {
        try
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return null;
            }

            // Update only provided fields
            if (updateTodoDto.UserId.HasValue)
            {
                // Verify user exists
                var user = await _context.Users.FindAsync(updateTodoDto.UserId.Value);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }
                todo.UserId = updateTodoDto.UserId.Value;
            }

            if (!string.IsNullOrEmpty(updateTodoDto.Title))
                todo.Title = updateTodoDto.Title;

            if (updateTodoDto.Completed.HasValue)
                todo.Completed = updateTodoDto.Completed.Value;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated todo with id {TodoId}", id);
            return _mapper.Map<TodoDto>(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating todo with id {TodoId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        try
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return false;
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted todo with id {TodoId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting todo with id {TodoId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TodoDto>> GetTodosByUserIdAsync(int userId)
    {
        try
        {
            var todos = await _context.Todos
                .Where(t => t.UserId == userId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<TodoDto>>(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todos for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<TodoDto>> GetCompletedTodosAsync()
    {
        try
        {
            var todos = await _context.Todos
                .Where(t => t.Completed)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<TodoDto>>(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting completed todos");
            throw;
        }
    }

    public async Task<IEnumerable<TodoDto>> GetPendingTodosAsync()
    {
        try
        {
            var todos = await _context.Todos
                .Where(t => !t.Completed)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<TodoDto>>(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting pending todos");
            throw;
        }
    }
} 