using BusinessObjects.Dtos.Auth;
using BusinessObjects;
using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Threading.Tasks;
using DataAccessLayers.UnitOfWork;
using System.Text.RegularExpressions;

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
            // Email format validation
            if (string.IsNullOrEmpty(registerDto.Email) || !IsValidEmail(registerDto.Email))
                throw new Exception("Invalid email format.");

            // Normalize email to lowercase for consistent comparisons
            registerDto.Email = registerDto.Email.Trim().ToLowerInvariant();

            // Check if email already exists - improved check
            var existingAccount = await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(registerDto.Email);
            if (existingAccount != null)
            {
                throw new Exception("Email is already registered.");
            }
            
            // Double-check with direct query to ensure email is unique
            try
            {
                // Password check
                if (string.IsNullOrEmpty(registerDto.Password) || registerDto.Password.Length < 6)
                    throw new Exception("Password must be at least 6 characters long.");

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
            catch (Exception ex) when (ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || 
                                      ex.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) || 
                                      ex.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Email is already registered.", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Simple email validation regex
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
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
