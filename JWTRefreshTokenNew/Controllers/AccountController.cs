using JWTRefreshTokenNew.Dtos;
using JWTRefreshTokenNew.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JWTRefreshTokenNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Route("login-token")]
        [HttpPost]
        public async Task<IActionResult> GetLoginToken(LoginDto login)
        {
            var result = await _accountService.GetAuthTokens(login);
            if (result == null)
            {
                return ValidationProblem("invalid credentials");
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("test-auth")]
        [Authorize]
        public IActionResult GetTest()
        {
            return Ok("Only authenticated user can consume this endpoint");
        }
        [HttpPost]
        [Route("renew-tokens")]
        public async Task<IActionResult> RenewTokens(RefreshTokenDto refreshToken)
        {
            var tokens = await _accountService.RenewTokens(refreshToken);
            if (tokens == null)
            {
                return ValidationProblem("Invalid Refresh Token");
            }
            return Ok(tokens);
        }
    }
}
