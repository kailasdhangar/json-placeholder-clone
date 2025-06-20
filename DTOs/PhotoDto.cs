using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
}

public class CreatePhotoDto
{
    [Required]
    public int AlbumId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;
    
    [Required]
    [Url]
    public string ThumbnailUrl { get; set; } = string.Empty;
}

public class UpdatePhotoDto
{
    public int? AlbumId { get; set; }
    
    [StringLength(200)]
    public string? Title { get; set; }
    
    [Url]
    public string? Url { get; set; }
    
    [Url]
    public string? ThumbnailUrl { get; set; }
} 