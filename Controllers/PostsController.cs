using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing posts")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    /// <summary>
    /// Get all posts
    /// </summary>
    /// <returns>List of all posts</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all posts", Description = "Retrieves a list of all posts")]
    [SwaggerResponse(200, "Successfully retrieved posts", typeof(IEnumerable<PostDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting posts");
            return StatusCode(500, "An error occurred while retrieving posts");
        }
    }

    /// <summary>
    /// Get a specific post by ID
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>Post details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get post by ID", Description = "Retrieves a specific post by its ID")]
    [SwaggerResponse(200, "Successfully retrieved post", typeof(PostDto))]
    [SwaggerResponse(404, "Post not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PostDto>> GetPost(int id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound($"Post with ID {id} not found");
            }
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting post with id {PostId}", id);
            return StatusCode(500, "An error occurred while retrieving the post");
        }
    }

    /// <summary>
    /// Create a new post
    /// </summary>
    /// <param name="createPostDto">Post data</param>
    /// <returns>Created post</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create post", Description = "Creates a new post")]
    [SwaggerResponse(201, "Post created successfully", typeof(PostDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
    {
        try
        {
            var post = await _postService.CreatePostAsync(createPostDto);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post");
            return StatusCode(500, "An error occurred while creating the post");
        }
    }

    /// <summary>
    /// Update an existing post
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <param name="updatePostDto">Updated post data</param>
    /// <returns>Updated post</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update post", Description = "Updates an existing post")]
    [SwaggerResponse(200, "Post updated successfully", typeof(PostDto))]
    [SwaggerResponse(404, "Post not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PostDto>> UpdatePost(int id, UpdatePostDto updatePostDto)
    {
        try
        {
            var post = await _postService.UpdatePostAsync(id, updatePostDto);
            if (post == null)
            {
                return NotFound($"Post with ID {id} not found");
            }
            return Ok(post);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post with id {PostId}", id);
            return StatusCode(500, "An error occurred while updating the post");
        }
    }

    /// <summary>
    /// Delete a post
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete post", Description = "Deletes a post")]
    [SwaggerResponse(204, "Post deleted successfully")]
    [SwaggerResponse(404, "Post not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var result = await _postService.DeletePostAsync(id);
            if (!result)
            {
                return NotFound($"Post with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post with id {PostId}", id);
            return StatusCode(500, "An error occurred while deleting the post");
        }
    }

    /// <summary>
    /// Get comments for a specific post
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>List of post's comments</returns>
    [HttpGet("{id}/comments")]
    [SwaggerOperation(Summary = "Get post comments", Description = "Retrieves all comments for a specific post")]
    [SwaggerResponse(200, "Successfully retrieved post comments", typeof(IEnumerable<CommentDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPostComments(int id)
    {
        try
        {
            var comments = await _postService.GetPostCommentsAsync(id);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comments for post {PostId}", id);
            return StatusCode(500, "An error occurred while retrieving post comments");
        }
    }

    /// <summary>
    /// Get a post with its comments
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>Post with comments</returns>
    [HttpGet("{id}/with-comments")]
    [SwaggerOperation(Summary = "Get post with comments", Description = "Retrieves a post with all its comments")]
    [SwaggerResponse(200, "Successfully retrieved post with comments", typeof(PostWithCommentsDto))]
    [SwaggerResponse(404, "Post not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PostWithCommentsDto>> GetPostWithComments(int id)
    {
        try
        {
            var post = await _postService.GetPostWithCommentsAsync(id);
            if (post == null)
            {
                return NotFound($"Post with ID {id} not found");
            }
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting post with comments for id {PostId}", id);
            return StatusCode(500, "An error occurred while retrieving the post with comments");
        }
    }
} 