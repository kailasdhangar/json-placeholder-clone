using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<PostDto>> GetUserPostsAsync(int userId);
    Task<IEnumerable<AlbumDto>> GetUserAlbumsAsync(int userId);
    Task<IEnumerable<TodoDto>> GetUserTodosAsync(int userId);
} 