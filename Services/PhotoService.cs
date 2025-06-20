using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class PhotoService : IPhotoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(ApplicationDbContext context, IMapper mapper, ILogger<PhotoService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PhotoDto>> GetAllPhotosAsync()
    {
        try
        {
            var photos = await _context.Photos.ToListAsync();
            return _mapper.Map<IEnumerable<PhotoDto>>(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all photos");
            throw;
        }
    }

    public async Task<PhotoDto?> GetPhotoByIdAsync(int id)
    {
        try
        {
            var photo = await _context.Photos.FindAsync(id);
            return _mapper.Map<PhotoDto>(photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photo with id {PhotoId}", id);
            throw;
        }
    }

    public async Task<PhotoDto> CreatePhotoAsync(CreatePhotoDto createPhotoDto)
    {
        try
        {
            // Verify album exists
            var album = await _context.Albums.FindAsync(createPhotoDto.AlbumId);
            if (album == null)
            {
                throw new InvalidOperationException("Album not found");
            }

            var photo = _mapper.Map<Photo>(createPhotoDto);
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new photo with id {PhotoId}", photo.Id);
            return _mapper.Map<PhotoDto>(photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating photo");
            throw;
        }
    }

    public async Task<PhotoDto?> UpdatePhotoAsync(int id, UpdatePhotoDto updatePhotoDto)
    {
        try
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return null;
            }

            // Update only provided fields
            if (updatePhotoDto.AlbumId.HasValue)
            {
                // Verify album exists
                var album = await _context.Albums.FindAsync(updatePhotoDto.AlbumId.Value);
                if (album == null)
                {
                    throw new InvalidOperationException("Album not found");
                }
                photo.AlbumId = updatePhotoDto.AlbumId.Value;
            }

            if (!string.IsNullOrEmpty(updatePhotoDto.Title))
                photo.Title = updatePhotoDto.Title;

            if (!string.IsNullOrEmpty(updatePhotoDto.Url))
                photo.Url = updatePhotoDto.Url;

            if (!string.IsNullOrEmpty(updatePhotoDto.ThumbnailUrl))
                photo.ThumbnailUrl = updatePhotoDto.ThumbnailUrl;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated photo with id {PhotoId}", id);
            return _mapper.Map<PhotoDto>(photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating photo with id {PhotoId}", id);
            throw;
        }
    }

    public async Task<bool> DeletePhotoAsync(int id)
    {
        try
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return false;
            }

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted photo with id {PhotoId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting photo with id {PhotoId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PhotoDto>> GetPhotosByAlbumIdAsync(int albumId)
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
} 