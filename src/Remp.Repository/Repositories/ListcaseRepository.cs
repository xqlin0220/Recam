using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.DataAccess.Data;

namespace Remp.Repository.Repositories
{
    public class ListcaseRepository : IListcaseRepository
    {
        private readonly AppDbContext _context;

        public ListcaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Listcase?> GetByIdAsync(int id)
        {
            return await _context.Listcases.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task UpdateAsync(Listcase listcase)
        {
            _context.Listcases.Update(listcase);
            await _context.SaveChangesAsync();
        }
    }
}