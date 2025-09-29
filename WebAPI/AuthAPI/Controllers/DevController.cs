using AuthAPI.Data;
using Common.Controllers;
using Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AuthAPI.Controllers;

[Route("api/dev")]
public class DevController(AuthDbContext _authDbContext) : BaseApiController
{
    [HttpGet]
    public ApiActionResult<string> Get() => Ok("DevController is running.");

    [HttpGet("download-schema")]
    public ApiActionResult<string> DownloadSchema()
    {
        var createScript = _authDbContext.Database.GenerateCreateScript();

        var fileContent = System.Text.Encoding.UTF8.GetBytes(createScript);

        var dbName = _authDbContext.Database.GetDbConnection().Database;
        var timestamp = DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
        var providerName = _authDbContext.Database.ProviderName ?? "unknown";

        var schemaFileName = $"{dbName}_{providerName}_schema_{timestamp}.sql";

        return File(fileContent, MediaTypeNames.Text.Plain, schemaFileName);
    }
}
