namespace Remp.Service.Interfaces;

public interface IMediaService
{
    Task DeleteAsync(int mediaId, string userId, string email, string role, string? ip, string? userAgent);
}