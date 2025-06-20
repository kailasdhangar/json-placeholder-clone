using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JsonPlaceholderClone.DTOs;
using JsonPlaceholderClone.Services;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for managing albums")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _albumService;
    private readonly ILogger<AlbumsController> _logger;

    public AlbumsController(IAlbumService albumService, ILogger<AlbumsController> logger)
    {
        _albumService = albumService;
        _logger = logger;
    }

    /// <summary>
    /// Get all albums
    /// </summary>
    /// <returns>List of all albums</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all albums", Description = "Retrieves a list of all albums")]
    [SwaggerResponse(200, "Successfully retrieved albums", typeof(IEnumerable<AlbumDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbums()
    {
        try
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return Ok(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting albums");
            return StatusCode(500, "An error occurred while retrieving albums");
        }
    }

    /// <summary>
    /// Get a specific album by ID
    /// </summary>
    /// <param name="id">Album ID</param>
    /// <returns>Album details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get album by ID", Description = "Retrieves a specific album by its ID")]
    [SwaggerResponse(200, "Successfully retrieved album", typeof(AlbumDto))]
    [SwaggerResponse(404, "Album not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<AlbumDto>> GetAlbum(int id)
    {
        try
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound($"Album with ID {id} not found");
            }
            return Ok(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting album with id {AlbumId}", id);
            return StatusCode(500, "An error occurred while retrieving the album");
        }
    }

    /// <summary>
    /// Create a new album
    /// </summary>
    /// <param name="createAlbumDto">Album data</param>
    /// <returns>Created album</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create album", Description = "Creates a new album")]
    [SwaggerResponse(201, "Album created successfully", typeof(AlbumDto))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<AlbumDto>> CreateAlbum(CreateAlbumDto createAlbumDto)
    {
        try
        {
            var album = await _albumService.CreateAlbumAsync(createAlbumDto);
            return CreatedAtAction(nameof(GetAlbum), new { id = album.Id }, album);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating album");
            return StatusCode(500, "An error occurred while creating the album");
        }
    }

    /// <summary>
    /// Update an existing album
    /// </summary>
    /// <param name="id">Album ID</param>
    /// <param name="updateAlbumDto">Updated album data</param>
    /// <returns>Updated album</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update album", Description = "Updates an existing album")]
    [SwaggerResponse(200, "Album updated successfully", typeof(AlbumDto))]
    [SwaggerResponse(404, "Album not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<AlbumDto>> UpdateAlbum(int id, UpdateAlbumDto updateAlbumDto)
    {
        try
        {
            var album = await _albumService.UpdateAlbumAsync(id, updateAlbumDto);
            if (album == null)
            {
                return NotFound($"Album with ID {id} not found");
            }
            return Ok(album);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating album with id {AlbumId}", id);
            return StatusCode(500, "An error occurred while updating the album");
        }
    }

    /// <summary>
    /// Delete an album
    /// </summary>
    /// <param name="id">Album ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete album", Description = "Deletes an album")]
    [SwaggerResponse(204, "Album deleted successfully")]
    [SwaggerResponse(404, "Album not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeleteAlbum(int id)
    {
        try
        {
            var result = await _albumService.DeleteAlbumAsync(id);
            if (!result)
            {
                return NotFound($"Album with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting album with id {AlbumId}", id);
            return StatusCode(500, "An error occurred while deleting the album");
        }
    }

    /// <summary>
    /// Get photos for a specific album
    /// </summary>
    /// <param name="id">Album ID</param>
    /// <returns>List of album's photos</returns>
    [HttpGet("{id}/photos")]
    [SwaggerOperation(Summary = "Get album photos", Description = "Retrieves all photos for a specific album")]
    [SwaggerResponse(200, "Successfully retrieved album photos", typeof(IEnumerable<PhotoDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAlbumPhotos(int id)
    {
        try
        {
            var photos = await _albumService.GetAlbumPhotosAsync(id);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting photos for album {AlbumId}", id);
            return StatusCode(500, "An error occurred while retrieving album photos");
        }
    }

    /// <summary>
    /// Get an album with its photos
    /// </summary>
    /// <param name="id">Album ID</param>
    /// <returns>Album with photos</returns>
    [HttpGet("{id}/with-photos")]
    [SwaggerOperation(Summary = "Get album with photos", Description = "Retrieves an album with all its photos")]
    [SwaggerResponse(200, "Successfully retrieved album with photos", typeof(AlbumWithPhotosDto))]
    [SwaggerResponse(404, "Album not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<AlbumWithPhotosDto>> GetAlbumWithPhotos(int id)
    {
        try
        {
            var album = await _albumService.GetAlbumWithPhotosAsync(id);
            if (album == null)
            {
                return NotFound($"Album with ID {id} not found");
            }
            return Ok(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting album with photos for id {AlbumId}", id);
            return StatusCode(500, "An error occurred while retrieving the album with photos");
        }
    }
} 