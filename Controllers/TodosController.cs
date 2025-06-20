using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing todos")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoService todoService, ILogger<TodosController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all todos
    /// </summary>
    /// <returns>List of all todos</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all todos", Description = "Retrieves a list of all todos")]
    [SwaggerResponse(200, "Successfully retrieved todos", typeof(IEnumerable<TodoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos()
    {
        try
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todos");
            return StatusCode(500, "An error occurred while retrieving todos");
        }
    }

    /// <summary>
    /// Get a specific todo by ID
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <returns>Todo details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get todo by ID", Description = "Retrieves a specific todo by its ID")]
    [SwaggerResponse(200, "Successfully retrieved todo", typeof(TodoDto))]
    [SwaggerResponse(404, "Todo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TodoDto>> GetTodo(int id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while retrieving the todo");
        }
    }

    /// <summary>
    /// Create a new todo
    /// </summary>
    /// <param name="createTodoDto">Todo data</param>
    /// <returns>Created todo</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create todo", Description = "Creates a new todo")]
    [SwaggerResponse(201, "Todo created successfully", typeof(TodoDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto createTodoDto)
    {
        try
        {
            var todo = await _todoService.CreateTodoAsync(createTodoDto);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating todo");
            return StatusCode(500, "An error occurred while creating the todo");
        }
    }

    /// <summary>
    /// Update an existing todo
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <param name="updateTodoDto">Updated todo data</param>
    /// <returns>Updated todo</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update todo", Description = "Updates an existing todo")]
    [SwaggerResponse(200, "Todo updated successfully", typeof(TodoDto))]
    [SwaggerResponse(404, "Todo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(int id, UpdateTodoDto updateTodoDto)
    {
        try
        {
            var todo = await _todoService.UpdateTodoAsync(id, updateTodoDto);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            return Ok(todo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    /// <summary>
    /// Delete a todo
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete todo", Description = "Deletes a todo")]
    [SwaggerResponse(204, "Todo deleted successfully")]
    [SwaggerResponse(404, "Todo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        try
        {
            var result = await _todoService.DeleteTodoAsync(id);
            if (!result)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while deleting the todo");
        }
    }

    /// <summary>
    /// Get todos by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of todos for the user</returns>
    [HttpGet("by-user/{userId}")]
    [SwaggerOperation(Summary = "Get todos by user ID", Description = "Retrieves all todos for a specific user")]
    [SwaggerResponse(200, "Successfully retrieved todos", typeof(IEnumerable<TodoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodosByUserId(int userId)
    {
        try
        {
            var todos = await _todoService.GetTodosByUserIdAsync(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todos for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving todos");
        }
    }

    /// <summary>
    /// Get completed todos
    /// </summary>
    /// <returns>List of completed todos</returns>
    [HttpGet("completed")]
    [SwaggerOperation(Summary = "Get completed todos", Description = "Retrieves all completed todos")]
    [SwaggerResponse(200, "Successfully retrieved completed todos", typeof(IEnumerable<TodoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetCompletedTodos()
    {
        try
        {
            var todos = await _todoService.GetCompletedTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting completed todos");
            return StatusCode(500, "An error occurred while retrieving completed todos");
        }
    }

    /// <summary>
    /// Get pending todos
    /// </summary>
    /// <returns>List of pending todos</returns>
    [HttpGet("pending")]
    [SwaggerOperation(Summary = "Get pending todos", Description = "Retrieves all pending todos")]
    [SwaggerResponse(200, "Successfully retrieved pending todos", typeof(IEnumerable<TodoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetPendingTodos()
    {
        try
        {
            var todos = await _todoService.GetPendingTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting pending todos");
            return StatusCode(500, "An error occurred while retrieving pending todos");
        }
    }
} 