using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ITokenService tokenService,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService= tokenService;
            _mapper= mapper;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
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

        /////////////////////////////////////////////////////////////
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
        //////////////////////////////////////////////////////
        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }
        }
        //////////////////////////////////////////////////////
        
        

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return;

            var code = Random.Shared.Next(1000, 10000).ToString();

            await _userManager.SetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCode",
                code);

            await _userManager.SetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCodeExpiry",
                DateTime.UtcNow.AddMinutes(2).ToString("O"));

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Reset Code",
                $"Your verification code is: {code}");
        }


        //////////////////////////////////////////////////////

        

        public async Task VerifyResetCodeAsync(VerifyCodeDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid email or verification code.");

            var savedCode = await _userManager.GetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCode");

            var expiry = await _userManager.GetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCodeExpiry");

            if (savedCode != dto.Code)
                throw new InvalidOperationException("Invalid verification code.");

            if (string.IsNullOrWhiteSpace(expiry))
                throw new InvalidOperationException("Verification code has expired.");

            if (DateTime.Parse(expiry) < DateTime.UtcNow)
                throw new InvalidOperationException("Verification code has expired.");

            await _userManager.SetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetVerified",
                "true");
        }

        //////////////////////////////////////////////////////

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid request.");

            var verified = await _userManager.GetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetVerified");

            if (verified != "true")
                throw new InvalidOperationException("Please verify the code first.");

            var expiry = await _userManager.GetAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCodeExpiry");

            if (string.IsNullOrWhiteSpace(expiry))
                throw new InvalidOperationException("Verification code has expired.");

            if (DateTime.Parse(expiry) < DateTime.UtcNow)
                throw new InvalidOperationException("Verification code has expired.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCode");

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetCodeExpiry");

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                "TourEgypt",
                "ResetVerified");
        }
        //////////////////////////////////////////////////////

       

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }
        public Task SendEmailConfirmationAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}

