using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JsonPlaceholderClone.Models;

public class Post
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    // Navigation properties
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
} 