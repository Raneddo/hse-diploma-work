using System;
using ADSD.Backend.Auth.Models;
using ADSD.Backend.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register/{link}")]
        public IActionResult RegisterByLink([FromRoute] string link, [FromBody] UserInfo userInfo)
        {
            throw new NotImplementedException();
            // return _authService.RegisterUserByLink(link, userInfo);
        }
        
    }
}