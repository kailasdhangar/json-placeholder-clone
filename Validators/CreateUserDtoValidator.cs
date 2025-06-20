using FluentValidation;
using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Phone)
            .Matches(@"^[\+]?[1-9][\d]{0,15}$").WithMessage("Phone must be a valid phone number");

        RuleFor(x => x.Website)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Website must be a valid URL");

        RuleFor(x => x.Address)
            .SetValidator(new AddressDtoValidator());

        RuleFor(x => x.Company)
            .SetValidator(new CompanyDtoValidator());
    }
}

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street cannot exceed 200 characters");

        RuleFor(x => x.Suite)
            .NotEmpty().WithMessage("Suite is required")
            .MaximumLength(100).WithMessage("Suite cannot exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required")
            .MaximumLength(20).WithMessage("Zipcode cannot exceed 20 characters");

        RuleFor(x => x.Geo)
            .SetValidator(new GeoDtoValidator());
    }
}

public class GeoDtoValidator : AbstractValidator<GeoDto>
{
    public GeoDtoValidator()
    {
        RuleFor(x => x.Lat)
            .NotEmpty().WithMessage("Latitude is required");

        RuleFor(x => x.Lng)
            .NotEmpty().WithMessage("Longitude is required");
    }
}

public class CompanyDtoValidator : AbstractValidator<CompanyDto>
{
    public CompanyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters");

        RuleFor(x => x.CatchPhrase)
            .MaximumLength(200).WithMessage("Catch phrase cannot exceed 200 characters");

        RuleFor(x => x.Bs)
            .MaximumLength(100).WithMessage("BS cannot exceed 100 characters");
    }
} 