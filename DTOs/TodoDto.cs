using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class TodoDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Completed { get; set; }
}

public class CreateTodoDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public bool Completed { get; set; }
}

public class UpdateTodoDto
{
    public int? UserId { get; set; }
    
    [StringLength(200)]
    public string? Title { get; set; }
    
    public bool? Completed { get; set; }
} 