using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;

namespace CrowdfundingAPI.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token, string refreshToken);
}