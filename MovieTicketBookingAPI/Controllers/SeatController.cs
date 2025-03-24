using BusinessObjects;
using BusinessObjects.Dtos.CinemaRoom;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Seat;
using BusinessObjects.Dtos.Ticket;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace MovieTicketBookingAPI.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatController(ISeatService seatService) : ControllerBase
    {
        private readonly ISeatService _seatService = seatService;


        [HttpGet("showtimeid/{showtimeId}/movieid/{movieId}")]
        public async Task<ActionResult<ResponseModel<IEnumerable<SeatDto>>>> GetAvailableSeatsByShowtimeId(int showtimeId, int movieId)
        {
            if (showtimeId <= 0 || movieId <= 0)
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Error = "Invalid showtimeId or movieId.",
                    ErrorCode = 400
                });

            try
            {
                var seats = await _seatService.GetAvailableSeatsByShowtimeId(showtimeId, movieId);

                if (seats == null || !seats.Any())
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Error = "No available seats found.",
                        ErrorCode = 404
                    });

                return Ok(new ResponseModel<IEnumerable<SeatDto>>
                {
                    Data = seats,
                    Success = true
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<SeatWithTicketsDto>>>> GetAll()
        {
            try
            {
                var seats = await _seatService.GetAllWithTickets();
                
                var seatDtos = seats.Select(seat => new SeatWithTicketsDto
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    CinemaRoomId = seat.CinemaRoomId,
                    CinemaRoom = seat.CinemaRoom != null ? new CinemaRoomDto 
                    {
                        Id = seat.CinemaRoom.Id,
                        RoomName = seat.CinemaRoom.RoomName,
                        Capacity = seat.CinemaRoom.Capacity
                    } : null,
                    Tickets = seat.Tickets.Select(ticket => new TicketDto
                    {
                        Id = ticket.Id,
                        SeatId = ticket.SeatId,
                        SeatName = seat.SeatNumber ?? string.Empty,
                        MovieName = ticket.Movie?.Name,
                        ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                        Price = ticket.Price,
                        Status = ticket.Status
                    }).ToList()
                }).ToList();
                
                return Ok(new ResponseModel<IEnumerable<SeatWithTicketsDto>>
                {
                    Data = seatDtos,
                    Success = true,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<SeatWithTicketsDto>>> GetById(int id)
        {
            try
            {
                var seat = await _seatService.GetByIdWithTickets(id);
                
                if (seat == null)
                {
                    return NotFound(new ResponseModel<SeatWithTicketsDto>
                    {
                        Success = false,
                        Error = $"Seat with ID {id} not found",
                        ErrorCode = 404
                    });
                }
                
                var seatDto = new SeatWithTicketsDto
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    CinemaRoomId = seat.CinemaRoomId,
                    CinemaRoom = seat.CinemaRoom != null ? new CinemaRoomDto 
                    {
                        Id = seat.CinemaRoom.Id,
                        RoomName = seat.CinemaRoom.RoomName,
                        Capacity = seat.CinemaRoom.Capacity
                    } : null,
                    Tickets = seat.Tickets.Select(ticket => new TicketDto
                    {
                        Id = ticket.Id,
                        SeatId = ticket.SeatId,
                        SeatName = seat.SeatNumber ?? string.Empty,
                        MovieName = ticket.Movie?.Name,
                        ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                        Price = ticket.Price,
                        Status = ticket.Status
                    }).ToList()
                };
                
                return Ok(new ResponseModel<SeatWithTicketsDto>
                {
                    Data = seatDto,
                    Success = true,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("cinemaroom/{cinemaRoomId}")]
        public async Task<ActionResult<ResponseModel<IEnumerable<SeatWithTicketsDto>>>> GetByCinemaRoomId(int cinemaRoomId)
        {
            try
            {
                var seats = await _seatService.GetByCinemaRoomIdWithTickets(cinemaRoomId);
                
                if (seats == null || !seats.Any())
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Error = $"No seats found for cinema room ID {cinemaRoomId}",
                        ErrorCode = 404
                    });
                }
                
                var seatDtos = seats.Select(seat => new SeatWithTicketsDto
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    CinemaRoomId = seat.CinemaRoomId,
                    CinemaRoom = seat.CinemaRoom != null ? new CinemaRoomDto 
                    {
                        Id = seat.CinemaRoom.Id,
                        RoomName = seat.CinemaRoom.RoomName,
                        Capacity = seat.CinemaRoom.Capacity
                    } : null,
                    Tickets = seat.Tickets.Select(ticket => new TicketDto
                    {
                        Id = ticket.Id,
                        SeatId = ticket.SeatId,
                        SeatName = seat.SeatNumber ?? string.Empty,
                        MovieName = ticket.Movie?.Name,
                        ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                        Price = ticket.Price,
                        Status = ticket.Status
                    }).ToList()
                }).ToList();
                
                return Ok(new ResponseModel<IEnumerable<SeatWithTicketsDto>>
                {
                    Data = seatDtos,
                    Success = true,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }
    }
}
