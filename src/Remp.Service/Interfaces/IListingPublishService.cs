using Remp.Service.DTOs;

namespace Remp.Service.Interfaces
{
    public interface IListingPublishService
    {
        Task<PublishListcaseResultDto> PublishAsync(int listcaseId);
    }
}