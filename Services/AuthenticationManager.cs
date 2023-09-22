using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        private User? _user; // user'a ait bilgileri içerecek

        public AuthenticationManager(ILoggerService logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        // Token oluşturma
        public async Task<string> CreateToken()
        {
            var signinCredentials = GetSigninCredentials();  // kullanıcının kimlik bilgilerini alma
            var claims = await GetClaims(); // kullanıcının hangi rolleri varsa alalım
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims);  // token oluşturma seçeneklerini ürettik
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions); // ilgili token'in oluşmasını sağlıyoruz
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);

            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles); // birden fazla role ekleme
            }
            return result;
        }

        // Kullanıcı doğrulama adımı
        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthenticationDto)
        {
            _user = await _userManager.FindByNameAsync(userForAuthenticationDto.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthenticationDto.Password));
            if(!result)
            {
                _logger.LogWarning($"{nameof(ValidateUser)} : Authentication failed. Wrong username or password.");
            }
            return result;
        }

        private SigningCredentials GetSigninCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);  // kullanılan algoritma bilgisi de geçildi
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _user.UserName) // listede bir tane eleman var, username bilgisini claims'e ekledik
            };

            var roles = await _userManager.GetRolesAsync(_user); // rolleri alıyoruz

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));  // her bir rolü claimse çeviriyoruz
            }

            return claims;
        }
        
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["validIssuer"],
                    audience: jwtSettings["validAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])), // Token'in geçerliliğini 60 dk olarak belirledik.
                    signingCredentials: signinCredentials);

            return tokenOptions;
        }
    }
}
