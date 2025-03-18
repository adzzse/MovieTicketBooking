using BusinessObjects;
using BusinessObjects.Dtos.Seat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class SeatRepository(MovieprojectContext context) : GenericRepository<Seat>(context)
    {
        public async Task<IEnumerable<SeatDto>> GetAvailableSeatsByShowtimeId(int showtimeId, int movieId)
        {
            var showtime = await _context.ShowTimes
                .Include(st => st.CinemaRoom)
                .ThenInclude(cr => cr.Seats)
                .FirstOrDefaultAsync(st => st.Id == showtimeId);

            if (showtime?.CinemaRoom == null)
                return Enumerable.Empty<SeatDto>();

            var bookedSeats = await _context.Tickets
                .Where(ticket => ticket.ShowtimeId == showtimeId && ticket.MovieId == movieId && ticket.Status != 0)
                .Select(ticket => ticket.SeatId)
                .ToListAsync();

            var availableSeats = showtime.CinemaRoom.Seats
                .Where(seat => !bookedSeats.Contains(seat.Id))
                .Select(seat => new SeatDto
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber ?? string.Empty,
                    CinemaRoomName = seat.CinemaRoom?.RoomName ?? string.Empty
                });

            return availableSeats;
        }

        public async Task<IEnumerable<Seat>> GetAllIncludeAsync()
        {
            return await _context.Seats.ToListAsync();
        }
    }
}
