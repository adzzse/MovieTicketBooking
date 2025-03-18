using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayers
{
    public class CategoryRepository(MovieprojectContext context) : GenericRepository<Category>(context)
    {
        public async Task<Category?> GetByCateName(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(a => a.Type == name);
        }
    }
}
