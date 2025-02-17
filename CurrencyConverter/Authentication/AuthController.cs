using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CurrencyConverter.Models;
using System.Security.Claims;
using System.Text;
using CurrencyConverter.Services;

namespace CurrencyConverter.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthProvider _authProvider;

        public AuthController()
        {
            _authProvider = AuthProviderFactory.GetAuthProvider("JWT");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin user)
        {
            return await _authProvider.GetBearerToken(user);
        }
    }
}
