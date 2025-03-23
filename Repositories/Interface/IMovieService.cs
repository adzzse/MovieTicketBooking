using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMovieService : IGenericService<Movie>
    {
        //Task<IEnumerable<Movie>> GetAllInclude();
        Task<IEnumerable<Movie>> GetAllIncludeType();
        
        // Add a method to get a movie by ID with the Category included
        Task<Movie> GetByIdIncludeType(int id);
    }
}
