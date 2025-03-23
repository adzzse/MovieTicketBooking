using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class MovieRepository(MovieprojectContext context) : GenericRepository<Movie>(context)
    {
        public async Task<IEnumerable<Movie>> GetAllIncludeType()
        {
            return await _context.Movies.Include(a => a.Category).ToListAsync();
        }
        
        public async Task<Movie> GetByIdIncludeType(int id)
        {
            return await _context.Movies
                .Include(a => a.Category)
                .Include(a => a.Tickets)
                    .ThenInclude(t => t.Showtime)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
