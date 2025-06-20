using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CreateCommentDto
{
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
}

public class UpdateCommentDto
{
    public int? PostId { get; set; }
    
    [StringLength(100)]
    public string? Name { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? Body { get; set; }
} 