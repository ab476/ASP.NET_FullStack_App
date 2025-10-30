using AuthAPI.Data;
using AuthAPI.Services.SqlSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SqlSchemaController(ISqlSchemaParser parser, ILogger<SqlSchemaController> logger, AuthDbContext authDbContext) : ControllerBase
{

    /// <summary>
    /// Generates the current EF Core model schema from DbContext and parses it.
    /// </summary>
    [HttpGet("parse-from-db")]
    [ProducesResponseType(typeof(List<SqlTable>), StatusCodes.Status200OK)]
    public IActionResult ParseSchemaFromDb()
    {
        try
        {
            var createScript = authDbContext.Database.GenerateCreateScript();

            var parsedSchema = parser.Parse(createScript);

            return Ok(parsedSchema);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing schema from DbContext.");
            return Problem("An error occurred while parsing the schema from the database context.");
        }
    }
}
