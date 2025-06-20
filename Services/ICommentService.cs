using JsonPlaceholderClone.DTOs;

namespace JsonPlaceholderClone.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
    Task<CommentDto?> GetCommentByIdAsync(int id);
    Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto);
    Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto updateCommentDto);
    Task<bool> DeleteCommentAsync(int id);
    Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId);
} 