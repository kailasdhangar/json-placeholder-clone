using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JsonPlaceholderClone.Models;

public class Todo
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public bool Completed { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
} 