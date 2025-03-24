using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Ticket;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Security.Claims;

namespace MovieTicketBookingAPI.Controllers
{
    public class BillCheckResponse
    {
        public bool IsValid { get; set; }
    }

    [ApiController]
    [Route("api/bills")]
    public class BillController(IBillService billService, IAuthService authService) : ControllerBase
    {
        private readonly IBillService _billService = billService;
        private readonly IAuthService _authService = authService;

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<BillDto>>>> GetAll()
        {
            try
            {
                var bills = await _billService.GetAll();
                
                // Check if bills list is empty and return 404 if it is
                if (bills == null || !bills.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<BillDto>>()
                    {
                        Data = null,
                        Error = "No bills found",
                        Success = false,
                        ErrorCode = 404
                    });
                }
                
                var billDtos = bills.Select(bill => MapToBillDto(bill)).ToList();
                
                return Ok(new ResponseModel<IEnumerable<BillDto>>()
                {
                    Data = billDtos,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<BillDto>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<BillDto>>> GetById(int id)
        {
            try
            {
                var bill = await _billService.GetById(id);
                if (bill == null)
                    return NotFound(new ResponseModel<BillDto>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });
                    
                var billDto = MapToBillDto(bill);
                
                return Ok(new ResponseModel<BillDto>()
                {
                    Data = billDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<BillDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("account/{accountId}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<BillDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<BillDto>>>> GetByAccountId(int accountId)
        {
            try
            {
                var bills = await _billService.GetBillsByAccountId(accountId);
                if (bills == null || !bills.Any())
                    return NotFound(new ResponseModel<IEnumerable<BillDto>>()
                    {
                        Data = null,
                        Error = $"No bills found for account {accountId}",
                        Success = false,
                        ErrorCode = 404
                    });
                    
                var billDtos = bills.Select(bill => MapToBillDto(bill)).ToList();
                
                return Ok(new ResponseModel<IEnumerable<BillDto>>()
                {
                    Data = billDtos,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<BillDto>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("check/{ticketId}")]
        [ProducesResponseType(typeof(ResponseModel<BillCheckResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<BillCheckResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<BillCheckResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<BillCheckResponse>>> CheckBill(int ticketId)
        {
            try
            {
                var result = await _billService.CheckBill(ticketId);
                return Ok(new ResponseModel<BillCheckResponse>()
                {
                    Data = new BillCheckResponse { IsValid = result },
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<BillCheckResponse>()
                {
                    Data = new BillCheckResponse { IsValid = false },
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<BillDto>>> Create([FromBody] Bill bill)
        {
            try
            {
                var createdBill = await _billService.Add(bill);
                var billDto = MapToBillDto(createdBill);
                
                return CreatedAtAction(nameof(GetById), new { id = createdBill.Id },
                    new ResponseModel<BillDto>()
                    {
                        Data = billDto,
                        Error = null,
                        Success = true,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<BillDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost("purchase")]
        [ProducesResponseType(typeof(ResponseModel<PurchaseTicketResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<PurchaseTicketResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<PurchaseTicketResponseDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseModel<PurchaseTicketResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<PurchaseTicketResponseDto>>> PurchaseTickets([FromBody] PurchaseTicketRequestDto request)
        {
            try
            {
                var response = await _billService.PurchaseTickets(request.ShowtimeId, request.SeatIds, request.UserId);
                return Ok(new ResponseModel<PurchaseTicketResponseDto>()
                {
                    Data = response,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<PurchaseTicketResponseDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<BillDto>>> Update(int id, [FromBody] Bill bill)
        {
            try
            {
                var existingBill = await _billService.GetById(id);
                if (existingBill == null)
                    return NotFound(new ResponseModel<BillDto>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                bill.Id = id;
                await _billService.Update(bill);
                var billDto = MapToBillDto(bill);
                
                return Ok(new ResponseModel<BillDto>()
                {
                    Data = billDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<BillDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<BillDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<BillDto>>> Delete(int id)
        {
            try
            {
                var existingBill = await _billService.GetById(id);
                if (existingBill == null)
                    return NotFound(new ResponseModel<BillDto>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                await _billService.Delete(id);
                var billDto = MapToBillDto(existingBill);
                
                return Ok(new ResponseModel<BillDto>()
                {
                    Data = billDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<BillDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }
        
        // Helper method to map Bill entity to BillDto
        private BillDto MapToBillDto(Bill bill)
        {
            return new BillDto
            {
                Id = bill.Id,
                AccountId = bill.AccountId,
                AccountName = bill.Account?.Name,
                TicketId = bill.TicketId,
                Ticket = bill.Ticket == null ? null : new TicketDto
                {
                    Id = bill.Ticket.Id,
                    SeatId = bill.Ticket.SeatId,
                    SeatName = bill.Ticket.Seat?.SeatNumber ?? string.Empty,
                    MovieName = bill.Ticket.Movie?.Name,
                    ShowDateTime = bill.Ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = bill.Ticket.Price,
                    Status = bill.Ticket.Status
                },
                Quantity = bill.Quantity,
                TotalPrice = bill.TotalPrice,
                PromotionId = bill.PromotionId,
                TransactionStatus = bill.Transaction?.Status
            };
        }
    }
}
