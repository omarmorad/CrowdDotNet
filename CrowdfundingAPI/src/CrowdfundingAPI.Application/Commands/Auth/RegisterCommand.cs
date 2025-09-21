using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;
using MediatR;

namespace CrowdfundingAPI.Application.Commands.Auth;

public class RegisterCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}