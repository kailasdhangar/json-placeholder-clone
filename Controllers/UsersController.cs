using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all users", Description = "Retrieves a list of all users")]
    [SwaggerResponse(200, "Successfully retrieved users", typeof(IEnumerable<UserDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting users");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieves a specific user by their ID")]
    [SwaggerResponse(200, "Successfully retrieved user", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user with id {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserDto">User data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create user", Description = "Creates a new user")]
    [SwaggerResponse(201, "User created successfully", typeof(UserDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(409, "User with email or username already exists")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">Updated user data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update user", Description = "Updates an existing user")]
    [SwaggerResponse(200, "User updated successfully", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with id {UserId}", id);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete user", Description = "Deletes a user")]
    [SwaggerResponse(204, "User deleted successfully")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound($"User with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with id {UserId}", id);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }

    /// <summary>
    /// Get posts for a specific user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user's posts</returns>
    [HttpGet("{id}/posts")]
    [SwaggerOperation(Summary = "Get user posts", Description = "Retrieves all posts for a specific user")]
    [SwaggerResponse(200, "Successfully retrieved user posts", typeof(IEnumerable<PostDto>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetUserPosts(int id)
    {
        try
        {
            var posts = await _userService.GetUserPostsAsync(id);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting posts for user {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving user posts");
        }
    }

    /// <summary>
    /// Get albums for a specific user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user's albums</returns>
    [HttpGet("{id}/albums")]
    [SwaggerOperation(Summary = "Get user albums", Description = "Retrieves all albums for a specific user")]
    [SwaggerResponse(200, "Successfully retrieved user albums", typeof(IEnumerable<AlbumDto>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetUserAlbums(int id)
    {
        try
        {
            var albums = await _userService.GetUserAlbumsAsync(id);
            return Ok(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting albums for user {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving user albums");
        }
    }

    /// <summary>
    /// Get todos for a specific user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user's todos</returns>
    [HttpGet("{id}/todos")]
    [SwaggerOperation(Summary = "Get user todos", Description = "Retrieves all todos for a specific user")]
    [SwaggerResponse(200, "Successfully retrieved user todos", typeof(IEnumerable<TodoDto>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetUserTodos(int id)
    {
        try
        {
            var todos = await _userService.GetUserTodosAsync(id);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting todos for user {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving user todos");
        }
    }
} 