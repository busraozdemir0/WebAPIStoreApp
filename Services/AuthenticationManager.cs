using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
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
using System.Security.Cryptography;
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
        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signinCredentials = GetSigninCredentials();  // kullanıcının kimlik bilgilerini alma
            var claims = await GetClaims(); // kullanıcının hangi rolleri varsa alalım
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims);  // token oluşturma seçeneklerini ürettik

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if(populateExp)
            {
                _user.RefreshTokenExpireTime = DateTime.Now.AddDays(7); // expire süresini 7 gün daha ileri ötelemiş olduk
            }

            await _userManager.UpdateAsync(_user); // işlemlerin veritabanına da yansıması için update ediyoruz
            
            var accessToken= new JwtSecurityTokenHandler().WriteToken(tokenOptions); // ilgili token'in oluşmasını sağlıyoruz

            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using(var rng=RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)  // süresi geçmiş olan token'dan bilgileri alıyoruz
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            var tokenValidationParamaters= new TokenValidationParameters
            {
                ValidateIssuer = true,  // bu key'i kim ürettiyse bunu doğrula
                ValidateAudience = true, // geçerli bir alıcı mı doğrular
                ValidateLifetime = true,  // geçerlilik süresi
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParamaters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;  // cast işlemi => securityToken'ı JwtSecurityToken'a çeviriyoruz
            
            if(jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) // çevirme işlemi başarılı mı diye kontrol
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return principal; // kullanıcı bilgilerini dönüyoruz
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken); // kullanıcı bilgilerini aldık
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);  // user tablosunda belirtilen kullanıcı var mı diye teyit ediyoruz

            if (user is null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)  // belirtilen user yoksa ya da refreshtoken eşit değilse
                throw new RefreshTokenBadRequestException();

            _user = user;
            return await CreateToken(populateExp: false);
        }
    }
}
