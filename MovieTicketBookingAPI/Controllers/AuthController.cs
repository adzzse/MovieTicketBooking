﻿using AutoMapper;
using BusinessObjects.Dtos.Auth;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects.Dtos.Schema_Response;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        #region Authentication Endpoints
        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseModel<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseModel<AuthResponseDto>>> Login(LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(new ResponseModel<AuthResponseDto>()
                {
                    Data = response,
                    Error = null,
                    ErrorCode = 200,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new ResponseModel<AuthResponseDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 401
                });
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseModel<Account>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Account>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Account>>> Register(RegisterDto registerDto)
        {
            try
            {
                registerDto.RoleId = 2;
                var newAccount = await _authService.Register(registerDto);
                return Ok(new ResponseModel<Account>()
                {
                    Data = newAccount,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AuthResponseDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }
        #endregion

        #region User Profile Endpoints
        [HttpGet("who-am-i")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<AccountResponseBasic>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<AccountResponseBasic>>> WhoAmI()
        {
            try
            {
                var account = await _authService.GetUserByClaims(HttpContext.User);
                return Ok(new ResponseModel<AccountResponseBasic>()
                {
                    Data = new AccountResponseBasic() {
                        Id = account.Id,
                        Name = account.Name,
                        Address = account.Address,
                        Phone = account.Phone,
                        Role = account.Role.Name,
                        Status = account.Status,
                        Email = account.Email,
                        Wallet = account.Wallet,
                    },
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Account>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }
        #endregion
    }
}
