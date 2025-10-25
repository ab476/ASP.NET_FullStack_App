using AuthAPI.Data;
using Common.Controllers;
using Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AuthAPI.Controllers;

[Route("api/dev")]
public class DevController(AuthDbContext _context, IMultiDatabaseConfig _databaseConfig) : BaseApiController
{
    [HttpGet("download-schema")]
    public FileResult DownloadSchema()
    {
        var createScript = _context.Database.GenerateCreateScript();

        var fileContent = System.Text.Encoding.UTF8.GetBytes(createScript);

        var dbName = _context.Database.GetDbConnection().Database;
        var timestamp = DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
        var providerName = _databaseConfig.ActiveDatabase.ToString() ?? "unknown";

        var schemaFileName = $"{dbName}_{providerName}_schema_{timestamp}.sql";

        return File(fileContent, MediaTypeNames.Text.Plain, schemaFileName);
    }

    [HttpGet]
    public ActionResult<ApiResponse<DevInfo>> Get([FromQuery] DevInfo devInfo)
    {
        return new ApiResponse<DevInfo>(true, devInfo);
    }
    private const string FilePath = "C:\\Users\\ariji\\Downloads\\permutations_0-5.csv";

    [HttpGet("GetFile")]
    public FileStreamResult Get()
    {
        var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return File(stream, "text/csv", Path.GetFileName(FilePath));
    }
}

public record DevInfo(string Application, string Environment, string Version);