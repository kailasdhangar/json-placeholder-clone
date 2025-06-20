using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class AlbumDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CreateAlbumDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
}

public class UpdateAlbumDto
{
    public int? UserId { get; set; }
    
    [StringLength(200)]
    public string? Title { get; set; }
}

public class AlbumWithPhotosDto : AlbumDto
{
    public List<PhotoDto> Photos { get; set; } = new();
} 