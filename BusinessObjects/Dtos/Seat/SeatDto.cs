using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessObjects.Dtos.CinemaRoom;
using BusinessObjects.Dtos.Ticket;

namespace BusinessObjects.Dtos.Seat
{
    public class SeatDto
    {
        [JsonPropertyName("seatId")]
        public int Id { get; set; }

        [JsonPropertyName("seatNumber")]
        public string? SeatNumber { get; set; }

        [JsonPropertyName("cinemaName")]
        public string? CinemaRoomName { get; set; }

        [JsonPropertyName("status")]
        public bool IsAvailable { get; set; } = true;
    }

    public class SeatWithTicketsDto
    {
        public int Id { get; set; }
        public string? SeatNumber { get; set; }
        public int CinemaRoomId { get; set; }
        public CinemaRoomDto? CinemaRoom { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
    }

    public class SeatSelectionDto
    {
        public int SeatId { get; set; }
        
        public int ShowtimeId { get; set; }
    }
}
