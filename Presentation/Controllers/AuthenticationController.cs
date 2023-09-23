using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController:ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]  // yazılan bu metod dto'nun boş olup olmamasını kontrol ediyor
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
        {
            var result = await _service.AuthenticationService.RegisterUser(userForRegistrationDto);

            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(201);  // hata yoksa 201 koduyla dönecek
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)  // login olmak için action
        {
            if(!await _service.AuthenticationService.ValidateUser(user))
            {
                return Unauthorized(); //401 
            }
            return Ok(new
            {
                Token = await _service.AuthenticationService.CreateToken()  // eğer başarılı bir şekilde giriş yapıldıysa o kullanıcıya ait token oluşturulacak
            });
        }

    }
}
