﻿using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuthenticationDto);  // Kullanıcı doğrulama işlemi için
        Task<TokenDto> CreateToken(bool populateExp);  // Token yaratmak için imza
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
