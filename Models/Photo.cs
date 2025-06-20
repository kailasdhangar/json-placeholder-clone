using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JsonPlaceholderClone.Models;

public class Photo
{
    public int Id { get; set; }
    
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
    
    // Navigation properties
    [JsonIgnore]
    public virtual Album Album { get; set; } = null!;
} 