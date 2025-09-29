using Common.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Shared.Tests.Controllers;
public class BaseApiControllerTests
{
    private class TestController : BaseApiController { }

    private readonly TestController _controller = new();

    [Fact]
    public void Ok_ShouldReturn_OkApiResult()
    {
        // Act
        var result = _controller.Ok();

        // Assert
        result.Should().BeOfType<OkApiResult>();
    }

    [Fact]
    public void Ok_WithValue_ShouldWrapInsideApiResponse()
    {
        // Arrange
        var value = "Hello";

        // Act
        var result = _controller.Ok(value);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okObject = result;
        okObject!.Value.Should().BeOfType<ApiResponse<object>>()
            .Which.Data.Should().Be(value);
    }

    [Fact]
    public void BadRequest_ShouldReturn_BadRequestApiResult()
    {
        // Act
        var result = _controller.BadRequest();

        // Assert
        result.Should().BeOfType<BadRequestApiResult>();
    }

    [Fact]
    public void BadRequest_WithError_ShouldWrapInsideApiResponse()
    {
        // Arrange
        var error = "Invalid Input";

        // Act
        var result = _controller.BadRequest(error);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result;
        badRequest!.Value.Should().BeOfType<ApiResponse<object>>()
            .Which.Data.Should().Be(error);
    }

    [Fact]
    public void BadRequest_Generic_ShouldWrapInsideApiResponse()
    {
        // Arrange
        var error = new { Code = "E001", Message = "Invalid data" };

        // Act
        var result = _controller.BadRequest(error);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result;
        badRequest!.Value.Should().BeOfType<ApiResponse<object>>(); // boxing happens here
    }

    [Fact]
    public void BadRequest_ModelState_ShouldWrapSerializableError()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Email", "Email is required");

        // Act
        var result = _controller.BadRequest(modelState);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result;
        badRequest!.Value.Should().BeOfType<ApiResponse<object>>()
            .Which.Data.Should().BeOfType<SerializableError>()
            .Which.Should().ContainKey("Email");
    }
}