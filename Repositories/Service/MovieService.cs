﻿using BusinessObjects;
using DataAccessLayers.UnitOfWork;
using Services.Interface;

namespace Services.Service
{
    public class MovieService(IUnitOfWork unitOfWork) : GenericService<Movie>(unitOfWork), IMovieService
    {
        //public async Task<IEnumerable<Movie>> GetAllInclude()
        //{
        //    return await _unitOfWork.EventRepository.GetAllInclude();
        //}
        public async Task<IEnumerable<Movie>> GetAllIncludeType()
        {
            return await _unitOfWork.MovieRepository.GetAllIncludeType();
        }
        
        public async Task<Movie> GetByIdIncludeType(int id)
        {
            return await _unitOfWork.MovieRepository.GetByIdIncludeType(id);
        }
    }
}
