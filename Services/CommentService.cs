using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CommentService> _logger;

    public CommentService(ApplicationDbContext context, IMapper mapper, ILogger<CommentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync()
    {
        try
        {
            var comments = await _context.Comments.ToListAsync();
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all comments");
            throw;
        }
    }

    public async Task<CommentDto?> GetCommentByIdAsync(int id)
    {
        try
        {
            var comment = await _context.Comments.FindAsync(id);
            return _mapper.Map<CommentDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comment with id {CommentId}", id);
            throw;
        }
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        try
        {
            // Verify post exists
            var post = await _context.Posts.FindAsync(createCommentDto.PostId);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found");
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new comment with id {CommentId}", comment.Id);
            return _mapper.Map<CommentDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating comment");
            throw;
        }
    }

    public async Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto updateCommentDto)
    {
        try
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }

            // Update only provided fields
            if (updateCommentDto.PostId.HasValue)
            {
                // Verify post exists
                var post = await _context.Posts.FindAsync(updateCommentDto.PostId.Value);
                if (post == null)
                {
                    throw new InvalidOperationException("Post not found");
                }
                comment.PostId = updateCommentDto.PostId.Value;
            }

            if (!string.IsNullOrEmpty(updateCommentDto.Name))
                comment.Name = updateCommentDto.Name;

            if (!string.IsNullOrEmpty(updateCommentDto.Email))
                comment.Email = updateCommentDto.Email;

            if (!string.IsNullOrEmpty(updateCommentDto.Body))
                comment.Body = updateCommentDto.Body;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated comment with id {CommentId}", id);
            return _mapper.Map<CommentDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating comment with id {CommentId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        try
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted comment with id {CommentId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting comment with id {CommentId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId)
    {
        try
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comments for post {PostId}", postId);
            throw;
        }
    }
} 