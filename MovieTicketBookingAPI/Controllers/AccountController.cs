using BusinessObjects;
using BusinessObjects.Dtos.Account;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using System.Net;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountService accountService, IAuthService authService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;
        private readonly IAuthService _authService = authService;

        #region Account Management Endpoints
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<AccountResponseBasic>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<AccountResponseBasic>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<AccountResponseBasic>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<AccountResponseBasic>>>> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAllIncludeAsync();
                
                // Check if accounts list is empty and return 404 if it is
                if (accounts == null || !accounts.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<AccountResponseBasic>> 
                    { 
                        Success = false, 
                        Error = "No accounts found", 
                        ErrorCode = 404 
                    });
                }
                
                var accountResponses = accounts.Select(account => new AccountResponseBasic
                {
                    Id = account.Id,
                    Name = account.Name,
                    Address = account.Address,
                    Phone = account.Phone,
                    Role = account.Role.Name,
                    Status = account.Status,
                    Email = account.Email,
                    Wallet = account.Wallet
                });
                return Ok(new ResponseModel<IEnumerable<AccountResponseBasic>> { Success = true, Data = accountResponses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<AccountResponseBasic>> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> GetById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdIncludeAsync(id);
                if (account == null)
                    return NotFound(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Account not found", ErrorCode = 404 });
                var accountResponse = new AccountResponseBasic
                {
                    Id = account.Id,
                    Name = account.Name,
                    Address = account.Address,
                    Phone = account.Phone,
                    Role = account.Role.Name,
                    Status = account.Status,
                    Email = account.Email,
                    Wallet = account.Wallet
                };

                return Ok(new ResponseModel<AccountResponseBasic> { Success = true, Data = accountResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AccountResponseBasic> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpPost("CreateAccount")]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> Create([FromBody] CreateAccountDto accountDto)
        {
            if (accountDto == null)
                return BadRequest(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Invalid account data", ErrorCode = 400 });
            try
            {
                var account = new Account
                {
                    Name = accountDto.Name,
                    Email = accountDto.Email,
                    Password = accountDto.Password,
                    Address = accountDto.Address,
                    Phone = accountDto.Phone,
                    RoleId = accountDto.RoleId,
                    Status = accountDto.Status,
                    Wallet = accountDto.Wallet
                };

                var createdAccount = await _accountService.Add(account);
                var accountResponse = new AccountResponseBasic
                {
                    Id = createdAccount.Id,
                    Name = createdAccount.Name,
                    Address = createdAccount.Address,
                    Phone = createdAccount.Phone,
                    Role = "Anonymous",
                    Status = createdAccount.Status,
                    Email = createdAccount.Email,
                    Wallet = createdAccount.Wallet
                };
                return CreatedAtAction(nameof(GetById), new { id = createdAccount.Id }, new ResponseModel<AccountResponseBasic> { Success = true, Data = accountResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AccountResponseBasic> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> Update(int id, [FromBody] AccountResponseBasic account)
        {
            if (account == null)
                return BadRequest(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Invalid account data", ErrorCode = 400 });

            try
            {
                var existingAccount = await _accountService.GetById(id);
                if (existingAccount == null)
                    return NotFound(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Account not found", ErrorCode = 404 });
                bool isUpdated = false;

                if (existingAccount.Name != account.Name)
                {
                    existingAccount.Name = account.Name;
                    isUpdated = true;
                }
                if (existingAccount.Address != account.Address)
                {
                    existingAccount.Address = account.Address;
                    isUpdated = true;
                }
                if (existingAccount.Phone != account.Phone)
                {
                    existingAccount.Phone = account.Phone;
                    isUpdated = true;
                }
                if (existingAccount.Status != account.Status)
                {
                    existingAccount.Status = account.Status;
                    isUpdated = true;
                }
                if (existingAccount.Email != account.Email)
                {
                    existingAccount.Email = account.Email;
                    isUpdated = true;
                }
                if (existingAccount.Wallet != account.Wallet)
                {
                    existingAccount.Wallet = (float)account.Wallet;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    await _accountService.Update(existingAccount);
                    return Ok(new ResponseModel<AccountResponseBasic> { Success = true, Data = account });
                }
                return Ok(new ResponseModel<AccountResponseBasic> { Success = false, Data = account, Error = "No changes detected" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AccountResponseBasic> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> Delete(int id)
        {
            try
            {
                var existingAccount = await _accountService.GetById(id);
                if (existingAccount == null)
                    return NotFound(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Account not found", ErrorCode = 404 });

                var accountResponse = new AccountResponseBasic
                {
                    Id = existingAccount.Id,
                    Name = existingAccount.Name,
                    Address = existingAccount.Address,
                    Phone = existingAccount.Phone,
                    Role = "Anonymous",
                    Status = existingAccount.Status,
                    Email = existingAccount.Email,
                    Wallet = existingAccount.Wallet
                };

                await _accountService.Delete(id);
                return Ok(new ResponseModel<AccountResponseBasic> { Success = true, Data = accountResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AccountResponseBasic> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }
        #endregion

        #region User Profile Endpoints
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ResponseModel<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<UserDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<UserDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<UserDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<UserDto>>> UpdateUser([FromBody] UserDto account)
        {
            var accountUser = await _authService.GetUserByClaims(HttpContext.User);
            if (account == null)
                return BadRequest(new ResponseModel<UserDto> { Success = false, Error = "Invalid account data", ErrorCode = 400 });

            try
            {
                var existingAccount = await _accountService.GetById(accountUser.Id);
                if (existingAccount == null)
                    return NotFound(new ResponseModel<UserDto> { Success = false, Error = "Account not found", ErrorCode = 404 });

                if (account.Name != "" && existingAccount.Name != account.Name)
                {
                    existingAccount.Name = account.Name;
                }
                if (account.Address != "" && existingAccount.Address != account.Address)
                {
                    existingAccount.Address = account.Address;
                }
                if (account.Phone != "" &&  existingAccount.Phone != account.Phone)
                {
                    existingAccount.Phone = account.Phone;
                }

                await _accountService.Update(existingAccount);
                return Ok(new ResponseModel<UserDto> { Success = true, Data = account });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<UserDto> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpPut("wallet")]
        [ProducesResponseType(typeof(ResponseModel<UserUpdateWalletDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<UserUpdateWalletDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<UserUpdateWalletDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<UserUpdateWalletDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<UserUpdateWalletDto>>> UpdateWallet([FromQuery] double wallet)
        {
            var accountUser = await _authService.GetUserByClaims(HttpContext.User);
            if (wallet < 0)
                return BadRequest(new ResponseModel<UserUpdateWalletDto> { Success = false, Error = "Invalid number", ErrorCode = 400 });
            try
            {
                var existingAccount = await _accountService.GetById(accountUser.Id);
                if (existingAccount == null)
                    return NotFound(new ResponseModel<UserUpdateWalletDto> { Success = false, Error = "Account not found", ErrorCode = 404 });
                existingAccount.Wallet += (float)wallet;

                await _accountService.Update(existingAccount);
                var updatedUserDto = new UserUpdateWalletDto
                {
                    Id = existingAccount.Id,
                    Name = existingAccount.Name,
                    Wallet = existingAccount.Wallet
                };
                return Ok(new ResponseModel<UserUpdateWalletDto> { Success = true, Data = updatedUserDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<UserUpdateWalletDto> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpPost("validation")]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> GetSystemAccountByEmailAndPassword([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                var account = await _accountService.GetSystemAccountByEmailAndPassword(email, password);
                if (account == null)
                    return BadRequest(new ResponseModel<AccountResponseBasic> { Success = false, Error = "Invalid credentials", ErrorCode = 400 });

                var accountResponse = new AccountResponseBasic
                {
                    Id = account.Id,
                    Name = account.Name,
                    Address = account.Address,
                    Phone = account.Phone,
                    Role = account.Role.Name,
                    Status = account.Status,
                    Email = account.Email,
                    Wallet = account.Wallet
                };

                return Ok(new ResponseModel<AccountResponseBasic> { Success = true, Data = accountResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AccountResponseBasic> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }
        #endregion
    }
}
