using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ADSD.Backend.App.Models;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ADSD.Backend.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
 
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
 
            return Json(response);
        }
 
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var userId = _authService.Login(username, password);
            var user = _userService.GetUser(userId);
            if (userId != default)
            {
                var claims = new List<Claim>
                {
                    new("id", userId.ToString()),
                    new(ClaimsIdentity.DefaultNameClaimType, username),
                    new(ClaimsIdentity.DefaultRoleClaimType, string.Join(',',user.Roles))
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
 
            return null;
        }
    }
}