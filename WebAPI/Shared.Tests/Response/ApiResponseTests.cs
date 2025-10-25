namespace Shared.Tests.Response;

public class ApiResponseTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        var success = true;
        var data = "Hello World";
        var message = "Operation successful";
        var statusCode = HttpStatusCode.OK;

        var response = new ApiResponse<string>(success, data, message, statusCode);

        response.Success.Should().BeTrue();
        response.Data.Should().Be(data);
        response.Message.Should().Be(message);
        response.StatusCode.Should().Be(statusCode);
    }

    [Fact]
    public void Constructor_ShouldAssignDefaultValues_WhenNotProvided()
    {
        var response = new ApiResponse<int>(true);

        response.Success.Should().BeTrue();
        response.Data.Should().Be(default);
        response.Message.Should().BeEmpty();
        response.StatusCode.Should().BeNull();
    }

    [Fact]
    public void Equality_ShouldCompareReferences_WhenClassType()
    {
        var response1 = new ApiResponse<int>(true, 42, "Ok");
        var response2 = new ApiResponse<int>(true, 42, "Ok");

        response1.Should().NotBeSameAs(response2);
        response1.Should().NotBe(response2);
    }

    [Fact]
    public void StatusCode_ShouldBeOptional()
    {
        var response = new ApiResponse<string>(false, "Error", "Something went wrong");

        response.StatusCode.Should().BeNull();
        response.Success.Should().BeFalse();
        response.Message.Should().Be("Something went wrong");
    }
}
