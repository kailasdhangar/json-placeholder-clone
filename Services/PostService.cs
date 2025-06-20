using AutoMapper;
using JsonPlaceholderClone.Data;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderClone.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PostService> _logger;

    public PostService(ApplicationDbContext context, IMapper mapper, ILogger<PostService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
    {
        try
        {
            var posts = await _context.Posts.ToListAsync();
            return _mapper.Map<IEnumerable<PostDto>>(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all posts");
            throw;
        }
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        try
        {
            var post = await _context.Posts.FindAsync(id);
            return _mapper.Map<PostDto>(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting post with id {PostId}", id);
            throw;
        }
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
    {
        try
        {
            // Verify user exists
            var user = await _context.Users.FindAsync(createPostDto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var post = _mapper.Map<Post>(createPostDto);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new post with id {PostId}", post.Id);
            return _mapper.Map<PostDto>(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post");
            throw;
        }
    }

    public async Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto)
    {
        try
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return null;
            }

            // Update only provided fields
            if (updatePostDto.UserId.HasValue)
            {
                // Verify user exists
                var user = await _context.Users.FindAsync(updatePostDto.UserId.Value);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }
                post.UserId = updatePostDto.UserId.Value;
            }

            if (!string.IsNullOrEmpty(updatePostDto.Title))
                post.Title = updatePostDto.Title;

            if (!string.IsNullOrEmpty(updatePostDto.Body))
                post.Body = updatePostDto.Body;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated post with id {PostId}", id);
            return _mapper.Map<PostDto>(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post with id {PostId}", id);
            throw;
        }
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        try
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted post with id {PostId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post with id {PostId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<CommentDto>> GetPostCommentsAsync(int postId)
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

    public async Task<PostWithCommentsDto?> GetPostWithCommentsAsync(int id)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            return _mapper.Map<PostWithCommentsDto>(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting post with comments for id {PostId}", id);
            throw;
        }
    }
} 