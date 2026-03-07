using Remp.Models.Entities;

namespace Remp.Repository.Interfaces
{
    public interface IListcaseRepository
    {
        Task<Listcase?> GetByIdAsync(int id);
    }
}