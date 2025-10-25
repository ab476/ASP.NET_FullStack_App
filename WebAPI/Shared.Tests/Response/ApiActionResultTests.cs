using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Shared.Tests.Response;

public class ApiActionResultTests
{
    [Fact]
    public void Constructor_WithValue_ShouldCreateSuccessResponse()
    {
        // Arrange
        var value = 42;
        var message = "Test message";

        // Act
        var result = new ApiActionResult<int>(value, message);

        // Assert
        result.Response.Should().NotBeNull();
        result.Response!.Success.Should().BeTrue();
        result.Response.Data.Should().Be(value);
        result.Response.Message.Should().Be(message);
        result.ActionResult.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithApiResponse_ShouldAssignResponse()
    {
        // Arrange
        var apiResponse = new ApiResponse<string>(true, "data", "ok");

        // Act
        var result = new ApiActionResult<string>(apiResponse);

        // Assert
        result.Response.Should().BeSameAs(apiResponse);
        result.ActionResult.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithActionResult_ShouldAssignResult()
    {
        // Arrange
        var actionResult = new NotFoundResult();

        // Act
        var result = new ApiActionResult<string>(actionResult);

        // Assert
        result.ActionResult.Should().BeSameAs(actionResult);
        result.Response.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateApiActionResult()
    {
        // Arrange
        int value = 123;

        // Act
        ApiActionResult<int> result = value;

        // Assert
        result.Response.Should().NotBeNull();
        result.Response!.Data.Should().Be(value);
        result.Response.Success.Should().BeTrue();
    }

    [Fact]
    public void ConvertToActionResult_ShouldReturnObjectResultWithCorrectStatus()
    {
        // Arrange
        var value = "Hello";
        var result = new ApiActionResult<string>(value);

        // Act
        var actionResult = ((IConvertToActionResult)result).Convert() as JsonResult;

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        ((ApiResponse<string>)actionResult.Value!).Data.Should().Be(value);
    }

    [Fact]
    public void ConvertToActionResult_WithActionResult_ShouldReturnActionResultDirectly()
    {
        // Arrange
        var actionResult = new BadRequestResult();
        var apiAction = new ApiActionResult<string>(actionResult);

        // Act
        var result = ((IConvertToActionResult)apiAction).Convert();

        // Assert
        result.Should().BeSameAs(actionResult);
    }

    [Fact]
    public void ImplicitConversion_FromTuple_ShouldCreateFailureResponse()
    {
        // Arrange
        var tuple = ("Error occurred", 0);

        // Act
        ApiActionResult<int> result = tuple;

        // Assert
        result.Response.Should().NotBeNull();
        result.Response!.Success.Should().BeFalse();
        result.Response.Data.Should().Be(0);
        result.Response.Message.Should().Be("Error occurred");
    }

    [Fact]
    public void ImplicitConversion_FromTuple_ShouldCreateSuccessResponse()
    {
        // Arrange
        var tuple = (123, "All good");

        // Act
        ApiActionResult<int> result = tuple;

        // Assert
        result.Response.Should().NotBeNull();
        result.Response!.Success.Should().BeTrue();
        result.Response.Data.Should().Be(123);
        result.Response.Message.Should().Be("All good");
    }
}

