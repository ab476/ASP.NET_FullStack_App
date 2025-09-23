namespace Shared.Tests.Response;

public class ApiResponseTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        // Arrange
        var success = true;
        var data = "Hello World";
        var message = "Operation successful";

        // Act
        var response = new ApiResponse<string>(success, data, message);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(data);
        response.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_ShouldAssignDefaultValues_WhenNotProvided()
    {
        // Act
        var response = new ApiResponse<int>(true);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(default(int));
        response.Message.Should().BeEmpty();
    }

    [Fact]
    public void Records_ShouldBeComparableByValue()
    {
        // Arrange
        var response1 = new ApiResponse<int>(true, 42, "Ok");
        var response2 = new ApiResponse<int>(true, 42, "Ok");

        // Assert
        response1.Should().Be(response2); // value-based equality
    }

    [Fact]
    public void Records_ShouldHaveDeconstructMethod()
    {
        // Arrange
        var response = new ApiResponse<string>(true, "Data", "Msg");

        // Act
        var (success, data, message) = response;

        // Assert
        success.Should().BeTrue();
        data.Should().Be("Data");
        message.Should().Be("Msg");
    }
}

