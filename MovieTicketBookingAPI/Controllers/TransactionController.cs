using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using Services.Service;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Transaction>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Transaction>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Transaction>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Transaction>>>> GetAll()
        {
            try
            {
                var transactions = await _transactionService.GetAll();
                
                // Check if transactions list is empty and return 404 if it is
                if (transactions == null || !transactions.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<Transaction>>()
                    {
                        Data = null,
                        Error = "No transactions found",
                        Success = false,
                        ErrorCode = 404
                    });
                }
                
                return Ok(new ResponseModel<IEnumerable<Transaction>>()
                {
                    Data = transactions,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<Transaction>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Transaction>>> GetById(int id)
        {
            try
            {
                var transaction = await _transactionService.GetById(id);
                if (transaction == null)
                    return NotFound(new ResponseModel<Transaction>()
                    {
                        Data = null,
                        Error = $"Not found transaction with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });
                return Ok(new ResponseModel<Transaction>()
                {
                    Data = transaction,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Transaction>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Transaction>>> Create([FromBody] Transaction transaction)
        {
            try
            {
                var createdTransaction = await _transactionService.Add(transaction);
                return CreatedAtAction(nameof(GetById), new { id = createdTransaction.Id },
                    new ResponseModel<Transaction>()
                    {
                        Data = createdTransaction,
                        Error = null,
                        Success = true,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Transaction>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Transaction>>> Update(int id, [FromBody] Transaction transaction)
        {
            try
            {
                var existingTransaction = await _transactionService.GetById(id);
                if (existingTransaction == null)
                    return NotFound(new ResponseModel<Transaction>()
                    {
                        Data = null,
                        Error = $"Not found transaction with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                transaction.Id = id;
                await _transactionService.Update(transaction);
                return Ok(new ResponseModel<Transaction>()
                {
                    Data = transaction,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Transaction>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Transaction>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Transaction>>> Delete(int id)
        {
            try
            {
                var existingTransaction = await _transactionService.GetById(id);
                if (existingTransaction == null)
                    return NotFound(new ResponseModel<Transaction>()
                    {
                        Data = null,
                        Error = $"Not found transaction with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                await _transactionService.Delete(id);
                return Ok(new ResponseModel<Transaction>()
                {
                    Data = existingTransaction,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Transaction>()
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
