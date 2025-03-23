using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.Ticket;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Net.Sockets;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/transactiontypes")]
    [ApiController]
    public class TransactionTypeController(ITransactionTypeService transactionTypeService) : ControllerBase
    {
        private readonly ITransactionTypeService _transactionTypeService = transactionTypeService;

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TransactionType>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TransactionType>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<TransactionType>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<TransactionType>>>> GetAll()
        {
            try
            {
                var transactionTypes = await _transactionTypeService.GetAll();
                
                // Check if transaction types list is empty and return 404 if it is
                if (transactionTypes == null || !transactionTypes.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<TransactionType>>
                    {
                        Success = false,
                        Error = "No transaction types found",
                        ErrorCode = 404
                    });
                }
                
                return Ok(new ResponseModel<IEnumerable<TransactionType>>
                {
                    Success = true,
                    Data = transactionTypes,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<TransactionType>>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TransactionType>>> GetById(int id)
        {
            try
            {
                var transactionType = await _transactionTypeService.GetById(id);
                if (transactionType == null)
                {
                    return NotFound(new ResponseModel<TransactionType>
                    {
                        Success = false,
                        Error = "Transaction type not found",
                        ErrorCode = 404
                    });
                }
                return Ok(new ResponseModel<TransactionType>
                {
                    Success = true,
                    Data = transactionType,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TransactionType> 
                { 
                    Success = false, 
                    Error = ex.Message, 
                    ErrorCode = 500 
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TransactionType>>> Create([FromBody] TransactionType transactionType)
        {
            try
            {
                var createdTransactionType = await _transactionTypeService.Add(transactionType);
                return CreatedAtAction(nameof(GetById), new { id = createdTransactionType.Id },
                    new ResponseModel<TransactionType>
                    {
                        Success = true,
                        Data = createdTransactionType,
                        Error = null,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TransactionType>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TransactionType>>> Update(int id, [FromBody] TransactionType transactionType)
        {
            try
            {
                var existingTransactionType = await _transactionTypeService.GetById(id);
                if (existingTransactionType == null)
                    return NotFound(new ResponseModel<TransactionType>
                    {
                        Success = false,
                        Error = "Transaction type not found",
                        ErrorCode = 404
                    });

                transactionType.Id = id;
                await _transactionTypeService.Update(transactionType);
                return Ok(new ResponseModel<TransactionType>
                {
                    Success = true,
                    Data = transactionType,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TransactionType>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<TransactionType>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<TransactionType>>> Delete(int id)
        {
            try
            {
                var existingTransactionType = await _transactionTypeService.GetById(id);
                if (existingTransactionType == null)
                    return NotFound(new ResponseModel<TransactionType>
                    {
                        Success = false,
                        Error = "Transaction type not found",
                        ErrorCode = 404
                    });

                await _transactionTypeService.Delete(id);
                return Ok(new ResponseModel<TransactionType>
                {
                    Success = true,
                    Data = existingTransactionType,
                    Error = null,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TransactionType>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }
    }
}
