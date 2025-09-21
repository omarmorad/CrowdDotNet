using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;
using MediatR;

namespace CrowdfundingAPI.Application.Commands.Auth;

public class LoginCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}