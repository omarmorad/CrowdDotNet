using CrowdfundingAPI.Application.Commands.Auth;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Auth;
using CrowdfundingAPI.Application.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerDto = new RegisterDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Bio = request.Bio
        };

        return await _authService.RegisterAsync(registerDto);
    }
}