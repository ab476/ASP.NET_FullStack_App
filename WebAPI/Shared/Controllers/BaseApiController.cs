using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    public override OkResult Ok()
    {
        return new OkApiResult();
    }
    public override OkObjectResult Ok(object value)
    {
        return base.Ok(new ApiResponse<object>(true, value));
    }
    public override BadRequestResult BadRequest()
    {
        return new BadRequestApiResult();
    }
    public override BadRequestObjectResult BadRequest(object error)
    {
        return new BadRequestObjectResult(new ApiResponse<object>(false, error, "Bad Request"));
    }
    public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
    {
        return new BadRequestObjectResult(new ApiResponse<object>(false, new SerializableError(modelState), "Bad Request"));
    }
}

internal class OkApiResult : OkResult
{
    public override Task ExecuteResultAsync(ActionContext context)
    {
        var result = new OkObjectResult(new ApiResponse<object>(true, null, "Ok Request"));
        return result.ExecuteResultAsync(context);
    }
}

internal class BadRequestApiResult : BadRequestResult
{
    public override Task ExecuteResultAsync(ActionContext context)
    {
        var result = new BadRequestObjectResult(new ApiResponse<object>(false, null, "Bad Request"));
        return result.ExecuteResultAsync(context);
    }
}