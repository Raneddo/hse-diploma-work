using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Models;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        [ProducesResponseType( typeof(RegisterUserResponse), 200)]
        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            var password = SecureHelper.GenerateRandomPassword();
            var passHash = SecureHelper.GenerateSecuredPassword(password, Encoding.Default.GetBytes("ADSD"));
            var userId = _authService.Register(request.UserName, passHash);
            _userService.CreateUser(new UserFullInfo
            {
                Id = userId,
                FirstName = "",
                LastName = "",
                Prefix = "",
                Organization = "",
                ApplicationStatus = "link sent",
                Email = request.Email,
                FullName = request.FullName,
                IsActive = false,
                Roles = new List<string> {UserRole.User.ToString()}
            });

            var secureData = HexadecimalEncoding
                .ToHexString($"{userId}\0{request.UserName}\0{password}");

            return Json(new RegisterUserResponse
            {
                Id = userId,
                Email = request.Email,
                UserName = request.UserName,
                SecureData = secureData,
            });
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public IActionResult Token([FromBody] UserCredentials userCredentials)
        {
            try
            {
                var identity = GetIdentity(userCredentials.UserName, userCredentials.Password);
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
            catch (InvalidUserException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut("/activate")]
        public IActionResult ActivateAccount([FromBody] UserCredentials userCredentials)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            _authService.ActivateAccount(userId.Value);
            var passHash = SecureHelper.GenerateSecuredPassword(userCredentials.Password,
                Encoding.Default.GetBytes("ADSD"));
            _userService.ChangeCredentials(userId.Value, userCredentials.UserName, passHash);
            return Ok();
        }

        [HttpGet("/activate/{secureData}")]
        public IActionResult ActivateByLink([FromRoute] string secureData)
        {
            var decoded = HexadecimalEncoding.FromHexString(secureData);
            var split = decoded.Split("\0");
            if (split.Length != 3)
            {
                return Unauthorized();
            }

            var userIdString = split[0];
            var userName = split[1];
            var password = split[2];

            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var user = _authService.Login(userName, password);
            if (user != userId)
            {
                return Unauthorized();
            }
            
            _authService.ActivateAccount(userId);
            return Json(_userService.GetUser(userId));
        }
 
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var userId = _authService.Login(username, password);
            var user = _userService.GetUser(userId);
            if (userId == default || user == null)
            {
                return null;
            }

            if (!user.IsActive && user.ApplicationStatus != "link sent")
            {
                throw new InvalidUserException("User is inactive");
            }

            var claims = new List<Claim>
            {
                new("id", userId.ToString()),
                new(ClaimsIdentity.DefaultNameClaimType, username),
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}