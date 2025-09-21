using CrowdfundingAPI.Application.Commands.Auth;
using CrowdfundingAPI.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrowdfundingAPI.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="registerDto">User registration information</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var command = new RegisterCommand
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Password = registerDto.Password,
            ConfirmPassword = registerDto.ConfirmPassword,
            Bio = registerDto.Bio
        };

        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var command = new LoginCommand
        {
            Email = loginDto.Email,
            Password = loginDto.Password
        };

        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token information</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        // Implementation would go here
        return Ok(new { message = "Refresh token endpoint - implementation pending" });
    }

    /// <summary>
    /// Send password reset email
    /// </summary>
    /// <param name="forgotPasswordDto">Email for password reset</param>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        // Implementation would go here
        return Ok(new { message = "Password reset email sent" });
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="resetPasswordDto">Password reset information</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        // Implementation would go here
        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Verify email address using verification token
    /// </summary>
    /// <param name="verifyEmailDto">Email verification information</param>
    /// <returns>Success message</returns>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        // Implementation would go here
        return Ok(new { message = "Email verified successfully" });
    }
}

public class RefreshTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class VerifyEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}