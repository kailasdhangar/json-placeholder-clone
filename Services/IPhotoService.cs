using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface IPhotoService
{
    Task<IEnumerable<PhotoDto>> GetAllPhotosAsync();
    Task<PhotoDto?> GetPhotoByIdAsync(int id);
    Task<PhotoDto> CreatePhotoAsync(CreatePhotoDto createPhotoDto);
    Task<PhotoDto?> UpdatePhotoAsync(int id, UpdatePhotoDto updatePhotoDto);
    Task<bool> DeletePhotoAsync(int id);
    Task<IEnumerable<PhotoDto>> GetPhotosByAlbumIdAsync(int albumId);
} 