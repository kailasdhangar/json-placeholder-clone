using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface IAlbumService
{
    Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
    Task<AlbumDto?> GetAlbumByIdAsync(int id);
    Task<AlbumDto> CreateAlbumAsync(CreateAlbumDto createAlbumDto);
    Task<AlbumDto?> UpdateAlbumAsync(int id, UpdateAlbumDto updateAlbumDto);
    Task<bool> DeleteAlbumAsync(int id);
    Task<IEnumerable<PhotoDto>> GetAlbumPhotosAsync(int albumId);
    Task<AlbumWithPhotosDto?> GetAlbumWithPhotosAsync(int id);
} 