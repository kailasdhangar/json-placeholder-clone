using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace JsonPlaceholderClone.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Test operations for debugging and development")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple GET test endpoint
    /// </summary>
    /// <returns>Test response</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Test GET endpoint", Description = "Returns a simple test response")]
    [SwaggerResponse(200, "Success", typeof(object))]
    public ActionResult<object> Get()
    {
        _logger.LogInformation("Test GET endpoint called");
        return Ok(new
        {
            message = "Test GET endpoint working!",
            timestamp = DateTime.UtcNow,
            status = "success"
        });
    }

    /// <summary>
    /// Test endpoint with parameter
    /// </summary>
    /// <param name="id">Test ID</param>
    /// <returns>Test response with ID</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Test GET with parameter", Description = "Returns test response with provided ID")]
    [SwaggerResponse(200, "Success", typeof(object))]
    [SwaggerResponse(400, "Bad request")]
    public ActionResult<object> GetById(int id)
    {
        _logger.LogInformation("Test GET endpoint called with ID: {Id}", id);
        
        if (id <= 0)
        {
            return BadRequest(new { error = "ID must be greater than 0" });
        }

        return Ok(new
        {
            message = $"Test GET endpoint working with ID: {id}!",
            id = id,
            timestamp = DateTime.UtcNow,
            status = "success"
        });
    }

    /// <summary>
    /// Test POST endpoint
    /// </summary>
    /// <param name="testData">Test data to process</param>
    /// <returns>Processed test data</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Test POST endpoint", Description = "Accepts test data and returns processed result")]
    [SwaggerResponse(201, "Created", typeof(object))]
    [SwaggerResponse(400, "Bad request")]
    public ActionResult<object> Post([FromBody] TestDataDto testData)
    {
        _logger.LogInformation("Test POST endpoint called with data: {Data}", JsonSerializer.Serialize(testData));
        
        if (testData == null)
        {
            return BadRequest(new { error = "Test data is required" });
        }

        if (string.IsNullOrWhiteSpace(testData.Name))
        {
            return BadRequest(new { error = "Name is required" });
        }

        var result = new
        {
            message = "Test POST endpoint working!",
            receivedData = testData,
            processedAt = DateTime.UtcNow,
            status = "success"
        };

        return CreatedAtAction(nameof(GetById), new { id = 1 }, result);
    }

    /// <summary>
    /// Test PUT endpoint
    /// </summary>
    /// <param name="id">Test ID</param>
    /// <param name="testData">Updated test data</param>
    /// <returns>Updated test data</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Test PUT endpoint", Description = "Updates test data for given ID")]
    [SwaggerResponse(200, "Updated", typeof(object))]
    [SwaggerResponse(400, "Bad request")]
    [SwaggerResponse(404, "Not found")]
    public ActionResult<object> Put(int id, [FromBody] TestDataDto testData)
    {
        _logger.LogInformation("Test PUT endpoint called with ID: {Id}", id);
        
        if (id <= 0)
        {
            return BadRequest(new { error = "ID must be greater than 0" });
        }

        if (testData == null)
        {
            return BadRequest(new { error = "Test data is required" });
        }

        // Simulate not found scenario
        if (id > 100)
        {
            return NotFound(new { error = $"Test data with ID {id} not found" });
        }

        var result = new
        {
            message = $"Test PUT endpoint working! Updated ID: {id}",
            id = id,
            updatedData = testData,
            updatedAt = DateTime.UtcNow,
            status = "success"
        };

        return Ok(result);
    }

    /// <summary>
    /// Test DELETE endpoint
    /// </summary>
    /// <param name="id">Test ID to delete</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Test DELETE endpoint", Description = "Deletes test data for given ID")]
    [SwaggerResponse(204, "Deleted successfully")]
    [SwaggerResponse(404, "Not found")]
    public ActionResult Delete(int id)
    {
        _logger.LogInformation("Test DELETE endpoint called with ID: {Id}", id);
        
        if (id <= 0)
        {
            return BadRequest(new { error = "ID must be greater than 0" });
        }

        // Simulate not found scenario
        if (id > 100)
        {
            return NotFound(new { error = $"Test data with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Test endpoint that throws an exception
    /// </summary>
    /// <returns>This will throw an exception</returns>
    [HttpGet("exception")]
    [SwaggerOperation(Summary = "Test exception handling", Description = "Intentionally throws an exception to test error handling")]
    [SwaggerResponse(500, "Internal server error")]
    public ActionResult<object> TestException()
    {
        _logger.LogWarning("Test exception endpoint called - this will throw an exception");
        throw new InvalidOperationException("This is a test exception to verify error handling");
    }

    /// <summary>
    /// Test endpoint with query parameters
    /// </summary>
    /// <param name="name">Name parameter</param>
    /// <param name="age">Age parameter</param>
    /// <param name="active">Active status</param>
    /// <returns>Test response with query parameters</returns>
    [HttpGet("query")]
    [SwaggerOperation(Summary = "Test query parameters", Description = "Accepts query parameters and returns them")]
    [SwaggerResponse(200, "Success", typeof(object))]
    public ActionResult<object> TestQueryParameters(
        [FromQuery] string? name = "default",
        [FromQuery] int age = 25,
        [FromQuery] bool active = true)
    {
        _logger.LogInformation("Test query parameters endpoint called with name: {Name}, age: {Age}, active: {Active}", name, age, active);

        return Ok(new
        {
            message = "Test query parameters endpoint working!",
            parameters = new
            {
                name = name,
                age = age,
                active = active
            },
            timestamp = DateTime.UtcNow,
            status = "success"
        });
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [SwaggerOperation(Summary = "Health check", Description = "Returns the health status of the API")]
    [SwaggerResponse(200, "Healthy", typeof(object))]
    public ActionResult<object> Health()
    {
        _logger.LogInformation("Health check endpoint called");
        
        int? number = null; // This is a palindrome
        string numberStr = number.ToString();
        string reversed = new string(numberStr.Reverse().ToArray());
        
        if (numberStr == reversed)
        {
            _logger.LogInformation("Palindrome check successful: {Number} is a palindrome", number);
            
            // Intentionally throw exception after successful palindrome check
            throw new InvalidOperationException($"Palindrome calculation completed but throwing exception for testing. Number {number} is indeed a palindrome.");
        }
        else
        {
            _logger.LogInformation("Palindrome check failed: {Number} is not a palindrome", number);
        }

        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        });
    }
}

/// <summary>
/// Test data DTO for POST and PUT operations
/// </summary>
public class TestDataDto
{
    /// <summary>
    /// Name of the test data
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the test data
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Value associated with the test data
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Whether the test data is active
    /// </summary>
    public bool IsActive { get; set; } = true;
} 