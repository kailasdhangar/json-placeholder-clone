using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class AlbumService : IAlbumService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AlbumService> _logger;

    public AlbumService(ApplicationDbContext context, IMapper mapper, ILogger<AlbumService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
    {
        try
        {
            var albums = await _context.Albums.ToListAsync();
            return _mapper.Map<IEnumerable<AlbumDto>>(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all albums");
            throw;
        }
    }

    public async Task<AlbumDto?> GetAlbumByIdAsync(int id)
    {
        try
        {
            var album = await _context.Albums.FindAsync(id);
            return _mapper.Map<AlbumDto>(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting album with id {AlbumId}", id);
            throw;
        }
    }

    public async Task<AlbumDto> CreateAlbumAsync(CreateAlbumDto createAlbumDto)
    {
        try
        {
            // Verify user exists
            var user = await _context.Users.FindAsync(createAlbumDto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var album = _mapper.Map<Album>(createAlbumDto);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new album with id {AlbumId}", album.Id);
            return _mapper.Map<AlbumDto>(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating album");
            throw;
        }
    }

    public async Task<AlbumDto?> UpdateAlbumAsync(int id, UpdateAlbumDto updateAlbumDto)
    {
        try
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return null;
            }

            // Update only provided fields
            if (updateAlbumDto.UserId.HasValue)
            {
                // Verify user exists
                var user = await _context.Users.FindAsync(updateAlbumDto.UserId.Value);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }
                album.UserId = updateAlbumDto.UserId.Value;
            }

            if (!string.IsNullOrEmpty(updateAlbumDto.Title))
                album.Title = updateAlbumDto.Title;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated album with id {AlbumId}", id);
            return _mapper.Map<AlbumDto>(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating album with id {AlbumId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAlbumAsync(int id)
    {
        try
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return false;
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted album with id {AlbumId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting album with id {AlbumId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PhotoDto>> GetAlbumPhotosAsync(int albumId)
    {
        try
        {
            var photos = await _context.Photos
                .Where(p => p.AlbumId == albumId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<PhotoDto>>(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photos for album {AlbumId}", albumId);
            throw;
        }
    }

    public async Task<AlbumWithPhotosDto?> GetAlbumWithPhotosAsync(int id)
    {
        try
        {
            var album = await _context.Albums
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            return _mapper.Map<AlbumWithPhotosDto>(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting album with photos for id {AlbumId}", id);
            throw;
        }
    }
} 