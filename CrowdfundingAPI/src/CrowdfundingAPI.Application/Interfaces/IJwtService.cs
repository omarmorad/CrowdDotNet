using System.Security.Claims;
using CrowdfundingAPI.Domain.Entities;

namespace CrowdfundingAPI.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}