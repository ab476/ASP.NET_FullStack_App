using Microsoft.AspNetCore.Mvc;


namespace AuthAPI.Controllers;

[Route("api/dev")]
public class DevController(AuthDbContext _context, IMultiDatabaseConfig _databaseConfig, ITimeProvider timeProvider) : ControllerBase
{
    [HttpGet("download-schema")]
    public FileResult DownloadSchema()
    {
        var createScript = _context.Database.GenerateCreateScript();

        var fileContent = Encoding.UTF8.GetBytes(createScript);

        var dbName = _context.Database.GetDbConnection().Database;
        var timestamp = timeProvider.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
        var providerName = _databaseConfig.ActiveDatabase.ToString() ?? "unknown";

        var schemaFileName = $"{dbName}_{providerName}_schema_{timestamp}.sql";

        return File(fileContent, MediaTypeNames.Text.Plain, schemaFileName);
    }
}