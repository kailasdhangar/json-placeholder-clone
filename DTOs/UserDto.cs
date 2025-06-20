using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderClone.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public CompanyDto Company { get; set; } = new();
}

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public AddressDto Address { get; set; } = new();
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public string Website { get; set; } = string.Empty;
    
    [Required]
    public CompanyDto Company { get; set; } = new();
}

public class UpdateUserDto
{
    [StringLength(100)]
    public string? Name { get; set; }
    
    [StringLength(100)]
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public AddressDto? Address { get; set; }
    
    [Phone]
    public string? Phone { get; set; }
    
    public string? Website { get; set; }
    
    public CompanyDto? Company { get; set; }
}

public class AddressDto
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
    
    public GeoDto Geo { get; set; } = new();
}

public class GeoDto
{
    [Required]
    public string Lat { get; set; } = string.Empty;
    
    [Required]
    public string Lng { get; set; } = string.Empty;
}

public class CompanyDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string CatchPhrase { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Bs { get; set; } = string.Empty;
} 