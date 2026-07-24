using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.DTOs.Auth;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IAuthService
    {

        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);

       
        Task SendEmailConfirmationAsync(string email);

        Task ConfirmEmailAsync(string userId, string token);

        Task ForgotPasswordAsync(string email);

        Task VerifyResetCodeAsync(VerifyCodeDto dto);

        Task ResetPasswordAsync(ResetPasswordDto dto);

        Task ChangePasswordAsync(ChangePasswordDto dto);



    }
}
