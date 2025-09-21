using AutoMapper;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs;
using CrowdfundingAPI.Application.DTOs.Auth;
using CrowdfundingAPI.Application.Interfaces;
using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Domain.Enums;
using CrowdfundingAPI.Domain.Interfaces;
using CrowdfundingAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CrowdfundingAPI.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResult("User with this email already exists");
        }

        // Create Identity user
        var identityUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(identityUser, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<AuthResponseDto>.ErrorResult("Failed to create user", errors);
        }

        // Create application user
        var appUser = new User
        {
            Id = identityUser.Id,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Bio = registerDto.Bio,
            Role = UserRole.User,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(appUser);
        identityUser.AppUserId = appUser.Id;
        await _userManager.UpdateAsync(identityUser);

        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var token = _jwtService.GenerateToken(appUser);
        var refreshToken = _jwtService.GenerateRefreshToken();

        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(identityUser);

        var userDto = _mapper.Map<UserDto>(appUser);
        var authResponse = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = userDto
        };

        return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "User registered successfully");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (identityUser == null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResult("Invalid email or password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, loginDto.Password);
        if (!isPasswordValid)
        {
            return ApiResponse<AuthResponseDto>.ErrorResult("Invalid email or password");
        }

        var appUser = await _unitOfWork.Users.GetByIdAsync(identityUser.AppUserId);
        if (appUser == null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResult("User profile not found");
        }

        // Update last login
        appUser.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(appUser);

        // Generate tokens
        var token = _jwtService.GenerateToken(appUser);
        var refreshToken = _jwtService.GenerateRefreshToken();

        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(identityUser);

        await _unitOfWork.SaveChangesAsync();

        var userDto = _mapper.Map<UserDto>(appUser);
        var authResponse = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = userDto
        };

        return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Login successful");
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token, string refreshToken)
    {
        // Implementation for refresh token
        return ApiResponse<AuthResponseDto>.ErrorResult("Refresh token functionality not implemented yet");
    }
}