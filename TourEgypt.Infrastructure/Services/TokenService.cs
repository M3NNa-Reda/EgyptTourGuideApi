using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TourEgypt.Core.Common;
using TourEgypt.Core.DTOs.Auth;
using TourEgypt.Core.DTOs.User;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IMapper _mapper;
        private readonly JwtOptions _jwtOptions;
        public TokenService(IMapper mapper,JwtOptions jwtOptions)
        {
            _mapper = mapper;
            _jwtOptions = jwtOptions;
        }
        public  Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user, IList<string> roles)
        {

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Signingkey)),
                SecurityAlgorithms.HmacSha256),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Lifetime),
                Subject = new ClaimsIdentity(claims)
            };
            var securityToken = tokenHandler.CreateToken(descriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            var response = new AuthResponseDto
            {
                Token = accessToken,
                Expiration = descriptor.Expires!.Value,
                User = _mapper.Map<UserDto>(user)
            };

            return Task.FromResult(response);
        }
    }
}
