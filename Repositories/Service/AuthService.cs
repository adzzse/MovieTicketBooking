﻿using BusinessObjects.Dtos.Auth;
using BusinessObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DataAccessLayers.UnitOfWork;

namespace Services.Service
{
    public class AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IAccountService accountService) : GenericService<Account>(unitOfWork), IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IAccountService _accountService = accountService;

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var account = await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(loginDto.Email);

            if (account == null || !VerifyPassword(loginDto.Password, account.Password ?? ""))
            {
                throw new UnauthorizedAccessException("Wrong email or password.");
            }

            var token = CreateToken(account);
            return new AuthResponseDto { Token = token };
        }

        public async Task<Account> Register(RegisterDto registerDto)
        {
            var existingAccount = await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(registerDto.Email);

            if (existingAccount != null)
            {
                throw new Exception("Email is already registered.");
            }

            var hashedPassword = HashPassword(registerDto.Password);
            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = hashedPassword,
                Name = registerDto.FullName,
                RoleId = registerDto.RoleId,
                Wallet = 0
            };

            await _accountService.Add(newAccount);
            return newAccount;
        }

        private string CreateToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, account.Email ?? throw new InvalidOperationException("Account email is null")),
                new(ClaimTypes.Role, account.Role?.Name ?? "User"),
                new("uid", account.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured"),
                audience: _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured"),
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_configuration["JwtSettings:DurationInDays"] ?? "7")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        public async Task<Account> GetUserByClaims(ClaimsPrincipal claims)
        {
            var userId = (claims.FindFirst(c => c.Type == "uid")?.Value) ?? throw new Exception("User not found.");
            var account = await _unitOfWork.AccountRepository.GetSystemAccountByIdIncludeRole(int.Parse(userId));

            return account ?? throw new Exception("User not found.");
        }

        public async Task<Account> GetAccountByEmail(string email)
        {
            return await _unitOfWork.AccountRepository.GetSystemAccountByAccountEmail(email);
        }
    }
}
