using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class PostDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CreatePostDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
}

public class UpdatePostDto
{
    public int? UserId { get; set; }
    
    [StringLength(200)]
    public string? Title { get; set; }
    
    public string? Body { get; set; }
}

public class PostWithCommentsDto : PostDto
{
    public List<CommentDto> Comments { get; set; } = new();
} 