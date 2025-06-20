using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, IMapper mapper, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await _context.Users
                .Include(u => u.Address.Geo)
                .Include(u => u.Company)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Address.Geo)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user with id {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            var user = _mapper.Map<User>(createUserDto);
            
            // Check if email or username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == createUserDto.Email || u.Username == createUserDto.Username);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email or username already exists");
            }
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new user with id {UserId}", user.Id);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw;
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Address.Geo)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return null;
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(updateUserDto.Name))
                user.Name = updateUserDto.Name;
            
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                user.Username = updateUserDto.Username;
            
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            
            if (!string.IsNullOrEmpty(updateUserDto.Phone))
                user.Phone = updateUserDto.Phone;
            
            if (!string.IsNullOrEmpty(updateUserDto.Website))
                user.Website = updateUserDto.Website;
            
            if (updateUserDto.Address != null)
            {
                if (!string.IsNullOrEmpty(updateUserDto.Address.Street))
                    user.Address.Street = updateUserDto.Address.Street;
                
                if (!string.IsNullOrEmpty(updateUserDto.Address.Suite))
                    user.Address.Suite = updateUserDto.Address.Suite;
                
                if (!string.IsNullOrEmpty(updateUserDto.Address.City))
                    user.Address.City = updateUserDto.Address.City;
                
                if (!string.IsNullOrEmpty(updateUserDto.Address.Zipcode))
                    user.Address.Zipcode = updateUserDto.Address.Zipcode;
                
                if (updateUserDto.Address.Geo != null)
                {
                    if (!string.IsNullOrEmpty(updateUserDto.Address.Geo.Lat))
                        user.Address.Geo.Lat = updateUserDto.Address.Geo.Lat;
                    
                    if (!string.IsNullOrEmpty(updateUserDto.Address.Geo.Lng))
                        user.Address.Geo.Lng = updateUserDto.Address.Geo.Lng;
                }
            }
            
            if (updateUserDto.Company != null)
            {
                if (!string.IsNullOrEmpty(updateUserDto.Company.Name))
                    user.Company.Name = updateUserDto.Company.Name;
                
                if (!string.IsNullOrEmpty(updateUserDto.Company.CatchPhrase))
                    user.Company.CatchPhrase = updateUserDto.Company.CatchPhrase;
                
                if (!string.IsNullOrEmpty(updateUserDto.Company.Bs))
                    user.Company.Bs = updateUserDto.Company.Bs;
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated user with id {UserId}", id);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with id {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted user with id {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with id {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PostDto>> GetUserPostsAsync(int userId)
    {
        try
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<PostDto>>(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting posts for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<AlbumDto>> GetUserAlbumsAsync(int userId)
    {
        try
        {
            var albums = await _context.Albums
                .Where(a => a.UserId == userId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<AlbumDto>>(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting albums for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<TodoDto>> GetUserTodosAsync(int userId)
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
} 