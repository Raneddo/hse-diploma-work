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
    [Route("/api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        public AuthController(AuthService authService, UserService userService, EmailService emailService)
        {
            _authService = authService;
            _userService = userService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [ProducesResponseType( typeof(RegisterUserResponse), 200)]
        [HttpPost("register")]
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

            _emailService.SendRegisterMessage(request.UserName, secureData, request.Email);
            
            return Json(new RegisterUserResponse
            {
                Id = userId,
                Email = request.Email,
                UserName = request.UserName,
                SecureData = secureData,
            });
        }

        [AllowAnonymous]
        [HttpPost("password/recover")]
        public IActionResult PasswordRecover(PasswordRecoverRequest request)
        {
            var userId = _authService.HardLogin(request.Email);
            var authUser = _authService.GetUser(userId);

            var password = SecureHelper.GenerateRandomPassword();
            var passHash = SecureHelper.GenerateSecuredPassword(password, Encoding.Default.GetBytes("ADSD"));
            
            _emailService.SendPasswordRecover(authUser.UserName, password, request.Email);
            _authService.ChangeCredentials(userId, authUser.UserName, passHash);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("password/change")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var authUser = _authService.GetUser(userId.Value);

            var loginUserId = _authService.Login(authUser.UserName, request.OldPassword);
            if (userId != loginUserId)
            {
                return Unauthorized("Invalid old password");
            }

            _authService.ChangeCredentials(userId.Value, authUser.UserName, request.NewPassword);
            return Ok();
        }
        

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Token([FromBody] UserCredentials userCredentials)
        {
            try
            {
                var userId = _authService.Login(userCredentials.UserName, userCredentials.Password);
                var identity = GetIdentity(userId);
                if (identity == null)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }
 
                var encodedJwt = GenerateJwt(identity);

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

        private static string GenerateJwt(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut("activate")]
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
            _authService.ChangeCredentials(userId.Value, userCredentials.UserName, passHash);
            return Ok();
        }

        [HttpGet("activate/{secureData}")]
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
 
        private ClaimsIdentity GetIdentity(int userId)
        {
            var user = _userService.GetUser(userId);
            var authUser = _authService.GetUser(userId);
            if (userId == default || user == null || authUser == null)
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
                new(ClaimsIdentity.DefaultNameClaimType, authUser.UserName),
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}