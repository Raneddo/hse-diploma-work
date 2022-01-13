using System.Threading.Tasks;
using ADSD.Backend.Auth.Models;
using ADSD.Backend.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADSD.Backend.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register/{link}")]
        public async Task<IActionResult> RegisterByLink([FromRoute] string link, [FromBody] UserInfo userInfo)
        {
            return Json(await _authService.RegisterUserByLink(link, userInfo));
        }

        [HttpPost("login/")]
        public async Task<IActionResult> Login([FromBody] UserPassPair userPassPair)
        {
            return Json(await _authService.Login(userPassPair.UserName, userPassPair.Password));
        }
        
    }
}