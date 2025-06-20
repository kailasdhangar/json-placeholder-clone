using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all comments
    /// </summary>
    /// <returns>List of all comments</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all comments", Description = "Retrieves a list of all comments")]
    [SwaggerResponse(200, "Successfully retrieved comments", typeof(IEnumerable<CommentDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
    {
        try
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comments");
            return StatusCode(500, "An error occurred while retrieving comments");
        }
    }

    /// <summary>
    /// Get a specific comment by ID
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Comment details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get comment by ID", Description = "Retrieves a specific comment by its ID")]
    [SwaggerResponse(200, "Successfully retrieved comment", typeof(CommentDto))]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found");
            }
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comment with id {CommentId}", id);
            return StatusCode(500, "An error occurred while retrieving the comment");
        }
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    /// <param name="createCommentDto">Comment data</param>
    /// <returns>Created comment</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create comment", Description = "Creates a new comment")]
    [SwaggerResponse(201, "Comment created successfully", typeof(CommentDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createCommentDto)
    {
        try
        {
            var comment = await _commentService.CreateCommentAsync(createCommentDto);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating comment");
            return StatusCode(500, "An error occurred while creating the comment");
        }
    }

    /// <summary>
    /// Update an existing comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <param name="updateCommentDto">Updated comment data</param>
    /// <returns>Updated comment</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update comment", Description = "Updates an existing comment")]
    [SwaggerResponse(200, "Comment updated successfully", typeof(CommentDto))]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int id, UpdateCommentDto updateCommentDto)
    {
        try
        {
            var comment = await _commentService.UpdateCommentAsync(id, updateCommentDto);
            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found");
            }
            return Ok(comment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating comment with id {CommentId}", id);
            return StatusCode(500, "An error occurred while updating the comment");
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete comment", Description = "Deletes a comment")]
    [SwaggerResponse(204, "Comment deleted successfully")]
    [SwaggerResponse(404, "Comment not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            var result = await _commentService.DeleteCommentAsync(id);
            if (!result)
            {
                return NotFound($"Comment with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting comment with id {CommentId}", id);
            return StatusCode(500, "An error occurred while deleting the comment");
        }
    }

    /// <summary>
    /// Get comments by post ID
    /// </summary>
    /// <param name="postId">Post ID</param>
    /// <returns>List of comments for the post</returns>
    [HttpGet("by-post/{postId}")]
    [SwaggerOperation(Summary = "Get comments by post ID", Description = "Retrieves all comments for a specific post")]
    [SwaggerResponse(200, "Successfully retrieved comments", typeof(IEnumerable<CommentDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId(int postId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comments for post {PostId}", postId);
            return StatusCode(500, "An error occurred while retrieving comments");
        }
    }
} 