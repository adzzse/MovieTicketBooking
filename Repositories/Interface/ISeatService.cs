using BusinessObjects;
using BusinessObjects.Dtos.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISeatService : IGenericService<Seat>
    {
        Task<IEnumerable<SeatDto>> GetAvailableSeatsByShowtimeId(int showtimeId, int movieId);
        Task<IEnumerable<Seat>> GetAllWithTickets();
        Task<Seat?> GetByIdWithTickets(int id);
        Task<IEnumerable<Seat>> GetByCinemaRoomIdWithTickets(int cinemaRoomId);
    }
}
