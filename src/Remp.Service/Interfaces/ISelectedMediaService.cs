using Remp.Service.DTOs;

namespace Remp.Service.Interfaces
{
    public interface ISelectedMediaService
    {
        Task<GetFinalSelectedMediaResponseDto> GetFinalSelectedMediaAsync(int listingId);
    }
}