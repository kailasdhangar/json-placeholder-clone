using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoDto>> GetAllTodosAsync();
    Task<TodoDto?> GetTodoByIdAsync(int id);
    Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto);
    Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto);
    Task<bool> DeleteTodoAsync(int id);
    Task<IEnumerable<TodoDto>> GetTodosByUserIdAsync(int userId);
    Task<IEnumerable<TodoDto>> GetCompletedTodosAsync();
    Task<IEnumerable<TodoDto>> GetPendingTodosAsync();
} 