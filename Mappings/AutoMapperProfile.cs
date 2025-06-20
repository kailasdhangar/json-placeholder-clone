using AutoMapper;
using JsonPlaceholderClone.Models;
using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UpdateUserDto>().ReverseMap();
        
        // Address mappings
        CreateMap<Address, AddressDto>().ReverseMap();
        
        // Geo mappings
        CreateMap<Geo, GeoDto>().ReverseMap();
        
        // Company mappings
        CreateMap<Company, CompanyDto>().ReverseMap();
        
        // Post mappings
        CreateMap<Post, PostDto>().ReverseMap();
        CreateMap<Post, CreatePostDto>().ReverseMap();
        CreateMap<Post, UpdatePostDto>().ReverseMap();
        CreateMap<Post, PostWithCommentsDto>()
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
        
        // Comment mappings
        CreateMap<Comment, CommentDto>().ReverseMap();
        CreateMap<Comment, CreateCommentDto>().ReverseMap();
        CreateMap<Comment, UpdateCommentDto>().ReverseMap();
        
        // Album mappings
        CreateMap<Album, AlbumDto>().ReverseMap();
        CreateMap<Album, CreateAlbumDto>().ReverseMap();
        CreateMap<Album, UpdateAlbumDto>().ReverseMap();
        CreateMap<Album, AlbumWithPhotosDto>()
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
        
        // Photo mappings
        CreateMap<Photo, PhotoDto>().ReverseMap();
        CreateMap<Photo, CreatePhotoDto>().ReverseMap();
        CreateMap<Photo, UpdatePhotoDto>().ReverseMap();
        
        // Todo mappings
        CreateMap<Todo, TodoDto>().ReverseMap();
        CreateMap<Todo, CreateTodoDto>().ReverseMap();
        CreateMap<Todo, UpdateTodoDto>().ReverseMap();
    }
} 