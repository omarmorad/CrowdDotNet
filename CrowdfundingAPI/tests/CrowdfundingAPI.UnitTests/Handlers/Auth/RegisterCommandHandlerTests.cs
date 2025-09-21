using CrowdfundingAPI.Application.Commands.Auth;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;
using CrowdfundingAPI.Application.Handlers.Auth;
using CrowdfundingAPI.Application.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrowdfundingAPI.UnitTests.Handlers.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new RegisterCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            Bio = "Test bio"
        };

        var expectedResponse = ApiResponse<AuthResponseDto>.SuccessResult(
            new AuthResponseDto
            {
                Token = "test-jwt-token",
                RefreshToken = "test-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new CrowdfundingAPI.Application.DTOs.UserDto
                {
                    Email = command.Email,
                    FirstName = command.FirstName,
                    LastName = command.LastName
                }
            },
            "User registered successfully");

        _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("test-jwt-token");
        result.Data.RefreshToken.Should().Be("test-refresh-token");
        result.Data.User.Should().NotBeNull();
        result.Data.User.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ShouldReturnErrorResponse()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "existing@example.com",
            Password = "Password123"
        };

        var expectedResponse = ApiResponse<AuthResponseDto>.ErrorResult("User with this email already exists");
        _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User with this email already exists");
    }
}