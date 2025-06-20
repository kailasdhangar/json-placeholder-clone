using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JsonPlaceholderClone.Models;

public class Comment
{
    public int Id { get; set; }
    
    [Required]
    public int PostId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    // Navigation properties
    [JsonIgnore]
    public virtual Post Post { get; set; } = null!;
} 