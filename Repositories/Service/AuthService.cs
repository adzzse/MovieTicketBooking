using BusinessObjects.Dtos.Auth;
using BusinessObjects;
using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Threading.Tasks;
using DataAccessLayers.UnitOfWork;

namespace Services.Service
{
    public class AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IAccountService accountService) : GenericService<Account>(unitOfWork), IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAccountService _accountService = accountService;

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var account = await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(loginDto.Email);
            if (account == null || account.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Wrong email or password.");

            return new AuthResponseDto { Token = "Success" };
        }

        public async Task<Account> Register(RegisterDto registerDto)
        {
            if (await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(registerDto.Email) != null)
                throw new Exception("Email is already registered.");

            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                Name = registerDto.FullName,
                RoleId = registerDto.RoleId,
            };

            await _accountService.Add(newAccount);
            return newAccount;
        }

        public async Task<Account> GetUserById(int userId)
        {
            var account = await _unitOfWork.AccountRepository.GetSystemAccountByIdIncludeRole(userId);
            return account ?? throw new Exception("User not found.");
        }

        public async Task<Account> GetAccountByEmail(string email)
        {
            return await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(email);
        }
    }
}
