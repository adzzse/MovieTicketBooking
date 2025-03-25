using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Transaction;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingAPI.Controllers
{
    [ApiController]
    [Route("api/transaction-history")]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService)
        {
            _transactionHistoryService = transactionHistoryService;
        }

        [HttpGet("user/{accountId}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TransactionHistoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<TransactionHistoryDto>>>> GetUserTransactionHistory(int accountId)
        {
            try
            {
                var transactionHistory = await _transactionHistoryService.GetUserTransactionHistory(accountId);
                
                if (transactionHistory == null || !transactionHistory.Any())
                {
                    return Ok(new ResponseModel<IEnumerable<TransactionHistoryDto>>
                    {
                        Success = true,
                        Data = new List<TransactionHistoryDto>(),
                        Error = null,
                        ErrorCode = 200
                    });
                }
                
                return Ok(new ResponseModel<IEnumerable<TransactionHistoryDto>>
                {
                    Success = true,
                    Data = transactionHistory,
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