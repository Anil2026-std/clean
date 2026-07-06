using ImageServer.dto;
using ImageServer.service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtSettings _jwtSettings;
        public AuthController(AuthService authService, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] LoginRequest request)
        {
            if (!_authService.ValidateClient(request.ClientId, request.ClientSecret))
                return Unauthorized("Invalid credentials.");

            var claims = new[] { new Claim("clientId", request.ClientId) };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = _jwtSettings.ExpiryMinutes * 60
            });
        }


    }
}
