using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class BillController(IBillService billService, IAuthService authService) : ControllerBase
    {
        private readonly IBillService _billService = billService;
        private readonly IAuthService _authService = authService;

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bill>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bill>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Bill>>>> GetAll()
        {
            try
            {
                var bills = await _billService.GetAll();
                return Ok(new ResponseModel<IEnumerable<Bill>>()
                {
                    Data = bills,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<Bill>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Bill>>> GetById(int id)
        {
            try
            {
                var bill = await _billService.GetById(id);
                if (bill == null)
                    return NotFound(new ResponseModel<Bill>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });
                return Ok(new ResponseModel<Bill>()
                {
                    Data = bill,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Bill>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("account/{accountId}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bill>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bill>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bill>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Bill>>>> GetByAccountId(int accountId)
        {
            try
            {
                var bills = await _billService.GetBillsByAccountId(accountId);
                if (bills == null || !bills.Any())
                    return NotFound(new ResponseModel<IEnumerable<Bill>>()
                    {
                        Data = null,
                        Error = $"No bills found for account {accountId}",
                        Success = false,
                        ErrorCode = 404
                    });
                return Ok(new ResponseModel<IEnumerable<Bill>>()
                {
                    Data = bills,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<Bill>>()
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Bill>>> Create([FromBody] Bill bill)
        {
            try
            {
                var createdBill = await _billService.Add(bill);
                return CreatedAtAction(nameof(GetById), new { id = createdBill.Id },
                    new ResponseModel<Bill>()
                    {
                        Data = createdBill,
                        Error = null,
                        Success = true,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Bill>()
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
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized(new ResponseModel<PurchaseTicketResponseDto>()
                    {
                        Data = null,
                        Error = "User not authenticated",
                        Success = false,
                        ErrorCode = 401
                    });
                }

                var account = await _authService.GetAccountByEmail(userEmail);
                if (account == null)
                {
                    return Unauthorized(new ResponseModel<PurchaseTicketResponseDto>()
                    {
                        Data = null,
                        Error = "User account not found",
                        Success = false,
                        ErrorCode = 401
                    });
                }

                var response = await _billService.PurchaseTickets(request.ShowtimeId, request.SeatIds, account);
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Bill>>> Update(int id, [FromBody] Bill bill)
        {
            try
            {
                var existingBill = await _billService.GetById(id);
                if (existingBill == null)
                    return NotFound(new ResponseModel<Bill>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                bill.Id = id;
                await _billService.Update(bill);
                return Ok(new ResponseModel<Bill>()
                {
                    Data = bill,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Bill>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Bill>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Bill>>> Delete(int id)
        {
            try
            {
                var existingBill = await _billService.GetById(id);
                if (existingBill == null)
                    return NotFound(new ResponseModel<Bill>()
                    {
                        Data = null,
                        Error = $"Not found bill with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                await _billService.Delete(id);
                return Ok(new ResponseModel<Bill>()
                {
                    Data = existingBill,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Bill>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }
    }
}
