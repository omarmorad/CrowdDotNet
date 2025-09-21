using CrowdfundingAPI.Application.Commands.Auth;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;
using CrowdfundingAPI.Application.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginDto = new LoginDto
        {
            Email = request.Email,
            Password = request.Password
        };

        return await _authService.LoginAsync(loginDto);
    }
}