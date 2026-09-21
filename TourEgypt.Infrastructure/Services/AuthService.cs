using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using TourEgypt.Core.DTOs.Auth;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private const string TokenProvider = "TourEgypt";

        private const string ResetCodeToken = "ResetCode";
        private const string ResetCodeExpiryToken = "ResetCodeExpiry";
        private const string ResetCodeAttemptsToken = "ResetCodeAttempts";
        private const string ResetVerifiedToken = "ResetVerified";
        private const string ResetCodeSentAtToken = "ResetCodeSentAt";

        private const int MaxResetCodeAttempts = 5;
        private const int ResetCodeExpiryMinutes = 2;
        private const int ResendCooldownSeconds = 60;
        private const int ResetSessionMinutes = 10;

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
            _tokenService = tokenService;
            _mapper = mapper;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already registered.");
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
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Registration failed: {errors}");
            }

            const string defaultRole = "User";

            var roleResult = await _userManager.AddToRoleAsync(
                newUser,
                defaultRole);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            var roles = new List<string> { defaultRole };

            return await _tokenService.GenerateTokenAsync(
                newUser,
                roles);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }


            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                throw new UnauthorizedAccessException(
                    "Account is temporarily locked due to too many failed attempts. Please try again later.");
            }

            if (!signInResult.Succeeded)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return await _tokenService.GenerateTokenAsync(
                user,
                roles);
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userIdStr =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) ||
                !int.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(
                userId.ToString());

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return;
            }

            
            var sentAtValue =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeSentAtToken);

            if (TryParseUtc(sentAtValue, out var sentAt) &&
                DateTime.UtcNow - sentAt < TimeSpan.FromSeconds(ResendCooldownSeconds))
            {
                return;
            }

            await ClearPasswordResetDataAsync(user);

            var code = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            var now = DateTime.UtcNow;
            var expiry = now.AddMinutes(ResetCodeExpiryMinutes);

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeToken,
                code);

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeExpiryToken,
                expiry.ToString("O"));

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeAttemptsToken,
                "0");

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeSentAtToken,
                now.ToString("O"));

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Password Reset Code",
                    $"Your verification code is: {code}");
            }
            catch
            {
                await ClearPasswordResetDataAsync(user);

                await _userManager.RemoveAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeSentAtToken);

                throw;
            }
        }

       
        public async Task VerifyResetCodeAsync(VerifyCodeDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Invalid email or verification code.");
            }

            var savedCode =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeToken);

            var expiryValue =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeExpiryToken);

            var attemptsValue =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeAttemptsToken);

            if (string.IsNullOrWhiteSpace(savedCode) ||
                string.IsNullOrWhiteSpace(expiryValue))
            {
                throw new InvalidOperationException(
                    "Verification code has expired.");
            }

            if (!TryParseUtc(expiryValue, out var expiry) ||
                expiry < DateTime.UtcNow)
            {
                await ClearPasswordResetDataAsync(user);

                throw new InvalidOperationException(
                    "Verification code has expired.");
            }

            var attempts = 0;

            if (!string.IsNullOrWhiteSpace(attemptsValue))
            {
                int.TryParse(attemptsValue, out attempts);
            }

            if (attempts >= MaxResetCodeAttempts)
            {
                await ClearPasswordResetDataAsync(user);

                throw new InvalidOperationException(
                    "Too many invalid attempts. Please request a new verification code.");
            }

            if (savedCode != dto.Code)
            {
                attempts++;

                if (attempts >= MaxResetCodeAttempts)
                {
                    await ClearPasswordResetDataAsync(user);

                    throw new InvalidOperationException(
                        "Too many invalid attempts. Please request a new verification code.");
                }

                await _userManager.SetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeAttemptsToken,
                    attempts.ToString());

                throw new InvalidOperationException(
                    "Invalid verification code.");
            }

           await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeToken);

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeExpiryToken,
                DateTime.UtcNow.AddMinutes(ResetSessionMinutes).ToString("O"));

            await _userManager.SetAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetVerifiedToken,
                "true");
        }

     
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Invalid request.");
            }

            var verified =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetVerifiedToken);

            if (verified != "true")
            {
                throw new InvalidOperationException(
                    "Please verify the code first.");
            }

            var expiryValue =
                await _userManager.GetAuthenticationTokenAsync(
                    user,
                    TokenProvider,
                    ResetCodeExpiryToken);

            if (!TryParseUtc(expiryValue, out var expiry) ||
                expiry < DateTime.UtcNow)
            {
                await ClearPasswordResetDataAsync(user);

                throw new InvalidOperationException(
                    "Your reset session has expired. Please request a new verification code.");
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(
                    user);

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            await ClearPasswordResetDataAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeSentAtToken);
        }
        private async Task ClearPasswordResetDataAsync(
            ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeToken);

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeExpiryToken);

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetCodeAttemptsToken);

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                TokenProvider,
                ResetVerifiedToken);
        }

        private static bool TryParseUtc(string? value, out DateTime result)
        {
            return DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out result);
        }

     
        public async Task ConfirmEmailAsync(
            string userId,
            string token)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailConfirmationAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}