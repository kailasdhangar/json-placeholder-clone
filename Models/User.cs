using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JsonPlaceholderClone.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public Address Address { get; set; } = new();
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public string Website { get; set; } = string.Empty;
    
    public Company Company { get; set; } = new();
    
    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    
    [JsonIgnore]
    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
    
    [JsonIgnore]
    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}

public class Address
{
    [Required]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Suite { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Zipcode { get; set; } = string.Empty;
    
    public Geo Geo { get; set; } = new();
}

public class Geo
{
    [Required]
    public string Lat { get; set; } = string.Empty;
    
    [Required]
    public string Lng { get; set; } = string.Empty;
}

public class Company
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string CatchPhrase { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Bs { get; set; } = string.Empty;
} 