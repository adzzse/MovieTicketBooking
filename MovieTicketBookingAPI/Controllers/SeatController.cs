﻿using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Seat;
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
        public async Task<ActionResult<ResponseModel<IEnumerable<Seat>>>> GetAll()
        {
            return Ok(await _seatService.GetAll());
        }
    }
}
