using BusinessObjects;
using BusinessObjects.Dtos.Count;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Ticket;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Data;
using System.Net.Sockets;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController(ITicketService ticketService) : ControllerBase
    {
        private readonly ITicketService _ticketService = ticketService;

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TicketDto>>> GetById(int id)
        {
            try
            {
                var ticket = await _ticketService.GetByIdInclude(id);
                if (ticket == null)
                {
                    return NotFound(new ResponseModel<TicketDto>
                    {
                        Success = false,
                        Error = "Ticket not found",
                        ErrorCode = 404
                    });
                }
                var ticketDto = new TicketDto
                {
                    Id = ticket.Id,
                    SeatId = ticket.SeatId,
                    SeatName = ticket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = ticket.Movie?.Name,
                    ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = ticket.Price,
                    Status = ticket.Status
                };
                return Ok(new ResponseModel<TicketDto>
                {
                    Success = true,
                    Data = ticketDto,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TicketDto> 
                { 
                    Success = false, 
                    Error = ex.Message, 
                    ErrorCode = 500 
                });
            }
        }

        [HttpGet("GetTicketsByMovieId/{movieId}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<TicketDto>>>> GetTicketsByMovieId(int movieId)
        {
            try
            {
                var tickets = await _ticketService.GetByMovieIdInclude(movieId);
                
                if (tickets == null || !tickets.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<TicketDto>>
                    {
                        Success = false,
                        Error = $"No tickets found for movie id {movieId}",
                        ErrorCode = 404
                    });
                }
                
                var ticketResponses = tickets.Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    SeatId = ticket.SeatId,
                    SeatName = ticket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = ticket.Movie?.Name,
                    ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = ticket.Price,
                    Status = ticket.Status
                }).ToList();
                
                return Ok(new ResponseModel<IEnumerable<TicketDto>>
                {
                    Success = true,
                    Data = ticketResponses,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<TicketDto>>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TicketDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<TicketDto>>>> GetAll()
        {
            try
            {
                var tickets = await _ticketService.GetAllIncludeAsync();
                
                if (tickets == null || !tickets.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<TicketDto>>
                    {
                        Success = false,
                        Error = "No tickets found",
                        ErrorCode = 404
                    });
                }
                
                var ticketResponses = tickets.Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    SeatId = ticket.SeatId,
                    SeatName = ticket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = ticket.Movie?.Name,
                    ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = ticket.Price,
                    Status = ticket.Status
                }).ToList();
                
                return Ok(new ResponseModel<IEnumerable<TicketDto>>
                {
                    Success = true,
                    Data = ticketResponses,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<TicketDto>>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TicketDto>>> AddTicket([FromBody] Ticket ticket)
        {
            try
            {
                var result = await _ticketService.Add(ticket);
                
                var ticketDto = new TicketDto
                {
                    Id = result.Id,
                    SeatId = result.SeatId,
                    SeatName = result.Seat?.SeatNumber ?? "Unknown",
                    MovieName = result.Movie?.Name,
                    ShowDateTime = result.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = result.Price,
                    Status = result.Status
                };
                
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, 
                    new ResponseModel<TicketDto>
                    {
                        Success = true,
                        Data = ticketDto,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TicketDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("ticket/{id}")]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TicketDto>>> UpdateNewTicket(int id, [FromBody] Ticket ticket)
        {
            try
            {
                var existingTicket = await _ticketService.GetById(id);
                if (existingTicket == null)
                {
                    return NotFound(new ResponseModel<TicketDto>
                    {
                        Success = false,
                        Error = $"Ticket with id {id} not found",
                        ErrorCode = 404
                    });
                }
                
                ticket.Id = id;
                await _ticketService.UpdateNewTicket(ticket);
                
                // Get the updated ticket with includes for proper mapping
                var updatedTicket = await _ticketService.GetByIdInclude(id);
                var ticketDto = new TicketDto
                {
                    Id = updatedTicket.Id,
                    SeatId = updatedTicket.SeatId,
                    SeatName = updatedTicket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = updatedTicket.Movie?.Name,
                    ShowDateTime = updatedTicket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = updatedTicket.Price,
                    Status = updatedTicket.Status
                };
                
                return Ok(new ResponseModel<TicketDto>
                {
                    Success = true,
                    Data = ticketDto,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TicketDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TicketDto>>> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            try
            {
                var existingTicket = await _ticketService.GetById(id);
                if (existingTicket == null)
                {
                    return NotFound(new ResponseModel<TicketDto>
                    {
                        Success = false,
                        Error = $"Ticket with id {id} not found",
                        ErrorCode = 404
                    });
                }

                ticket.Id = id;
                await _ticketService.Update(ticket);
                
                // Get the updated ticket with includes for proper mapping
                var updatedTicket = await _ticketService.GetByIdInclude(id);
                var ticketDto = new TicketDto
                {
                    Id = updatedTicket.Id,
                    SeatId = updatedTicket.SeatId,
                    SeatName = updatedTicket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = updatedTicket.Movie?.Name,
                    ShowDateTime = updatedTicket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = updatedTicket.Price,
                    Status = updatedTicket.Status
                };
                
                return Ok(new ResponseModel<TicketDto>
                {
                    Success = true,
                    Data = ticketDto,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TicketDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TicketDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TicketDto>>> DeleteTicket(int id)
        {
            try
            {
                var existingTicket = await _ticketService.GetByIdInclude(id);
                if (existingTicket == null)
                {
                    return NotFound(new ResponseModel<TicketDto>
                    {
                        Success = false,
                        Error = $"Ticket with id {id} not found",
                        ErrorCode = 404
                    });
                }
                
                var ticketDto = new TicketDto
                {
                    Id = existingTicket.Id,
                    SeatId = existingTicket.SeatId,
                    SeatName = existingTicket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = existingTicket.Movie?.Name,
                    ShowDateTime = existingTicket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = existingTicket.Price,
                    Status = existingTicket.Status
                };

                await _ticketService.Delete(id);
                
                return Ok(new ResponseModel<TicketDto>
                {
                    Success = true,
                    Data = ticketDto,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TicketDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }
    }
}
