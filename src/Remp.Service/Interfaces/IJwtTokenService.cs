namespace Remp.Service.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expiresUtc) CreateToken(string userId, string email, string role);
}