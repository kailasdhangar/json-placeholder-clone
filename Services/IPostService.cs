using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface IPostService
{
    Task<IEnumerable<PostDto>> GetAllPostsAsync();
    Task<PostDto?> GetPostByIdAsync(int id);
    Task<PostDto> CreatePostAsync(CreatePostDto createPostDto);
    Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto);
    Task<bool> DeletePostAsync(int id);
    Task<IEnumerable<CommentDto>> GetPostCommentsAsync(int postId);
    Task<PostWithCommentsDto?> GetPostWithCommentsAsync(int id);
} 