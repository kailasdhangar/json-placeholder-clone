using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing photos")]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;
    private readonly ILogger<PhotosController> _logger;

    public PhotosController(IPhotoService photoService, ILogger<PhotosController> logger)
    {
        _photoService = photoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all photos
    /// </summary>
    /// <returns>List of all photos</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all photos", Description = "Retrieves a list of all photos")]
    [SwaggerResponse(200, "Successfully retrieved photos", typeof(IEnumerable<PhotoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotos()
    {
        try
        {
            var photos = await _photoService.GetAllPhotosAsync();
            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photos");
            return StatusCode(500, "An error occurred while retrieving photos");
        }
    }

    /// <summary>
    /// Get a specific photo by ID
    /// </summary>
    /// <param name="id">Photo ID</param>
    /// <returns>Photo details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get photo by ID", Description = "Retrieves a specific photo by its ID")]
    [SwaggerResponse(200, "Successfully retrieved photo", typeof(PhotoDto))]
    [SwaggerResponse(404, "Photo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PhotoDto>> GetPhoto(int id)
    {
        try
        {
            var photo = await _photoService.GetPhotoByIdAsync(id);
            if (photo == null)
            {
                return NotFound($"Photo with ID {id} not found");
            }
            return Ok(photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photo with id {PhotoId}", id);
            return StatusCode(500, "An error occurred while retrieving the photo");
        }
    }

    /// <summary>
    /// Create a new photo
    /// </summary>
    /// <param name="createPhotoDto">Photo data</param>
    /// <returns>Created photo</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create photo", Description = "Creates a new photo")]
    [SwaggerResponse(201, "Photo created successfully", typeof(PhotoDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PhotoDto>> CreatePhoto(CreatePhotoDto createPhotoDto)
    {
        try
        {
            var photo = await _photoService.CreatePhotoAsync(createPhotoDto);
            return CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, photo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating photo");
            return StatusCode(500, "An error occurred while creating the photo");
        }
    }

    /// <summary>
    /// Update an existing photo
    /// </summary>
    /// <param name="id">Photo ID</param>
    /// <param name="updatePhotoDto">Updated photo data</param>
    /// <returns>Updated photo</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update photo", Description = "Updates an existing photo")]
    [SwaggerResponse(200, "Photo updated successfully", typeof(PhotoDto))]
    [SwaggerResponse(404, "Photo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PhotoDto>> UpdatePhoto(int id, UpdatePhotoDto updatePhotoDto)
    {
        try
        {
            var photo = await _photoService.UpdatePhotoAsync(id, updatePhotoDto);
            if (photo == null)
            {
                return NotFound($"Photo with ID {id} not found");
            }
            return Ok(photo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating photo with id {PhotoId}", id);
            return StatusCode(500, "An error occurred while updating the photo");
        }
    }

    /// <summary>
    /// Delete a photo
    /// </summary>
    /// <param name="id">Photo ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete photo", Description = "Deletes a photo")]
    [SwaggerResponse(204, "Photo deleted successfully")]
    [SwaggerResponse(404, "Photo not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        try
        {
            var result = await _photoService.DeletePhotoAsync(id);
            if (!result)
            {
                return NotFound($"Photo with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting photo with id {PhotoId}", id);
            return StatusCode(500, "An error occurred while deleting the photo");
        }
    }

    /// <summary>
    /// Get photos by album ID
    /// </summary>
    /// <param name="albumId">Album ID</param>
    /// <returns>List of photos for the album</returns>
    [HttpGet("by-album/{albumId}")]
    [SwaggerOperation(Summary = "Get photos by album ID", Description = "Retrieves all photos for a specific album")]
    [SwaggerResponse(200, "Successfully retrieved photos", typeof(IEnumerable<PhotoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosByAlbumId(int albumId)
    {
        try
        {
            var photos = await _photoService.GetPhotosByAlbumIdAsync(albumId);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photos for album {AlbumId}", albumId);
            return StatusCode(500, "An error occurred while retrieving photos");
        }
    }
} 