using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Auth;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService= tokenService;
            _mapper= mapper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already registered!");
            }

            var names = dto.FullName.Trim().Split(" ", 2);
            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : string.Empty;

            var newUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = dto.Phone
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            const string defaultRole = "User";
            var roleResult = await _userManager.AddToRoleAsync(newUser, defaultRole);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }
            var roles = new List<string> { defaultRole };

            return await _tokenService.GenerateTokenAsync(newUser, roles);
        }


        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }
            var roles=await _userManager.GetRolesAsync(user);
            

            return await _tokenService.GenerateTokenAsync(user, roles);

        }

        public Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task ForgotPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }      
        
        public Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailConfirmationAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task VerifyResetCodeAsync(VerifyCodeDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
