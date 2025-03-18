﻿using BusinessObjects;
using BusinessObjects.Dtos.ShowTime;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class ShowTimeRepository(Prn221projectContext context) : GenericRepository<ShowTime>(context)
    {
        public async Task<List<ShowtimeDto>> GetShowtimesByMovieId(int movieId)
        {
            return await _context.ShowTimes
                .Where(s => s.MovieId == movieId)
                .Include(s => s.CinemaRoom)
                .Select(s => new ShowtimeDto
                {
                    Id = s.Id,
                    RoomName = s.CinemaRoom.RoomName,
                    ShowDateTime = s.ShowDateTime
                }).ToListAsync();
        }

        public async Task<ShowTime?> GetByIdIncludeAsync(int id)
        {
            return await _context.ShowTimes
                .Include(s => s.CinemaRoom)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
