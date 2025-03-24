using Application.Services;
using DTO.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class AuthenticateController : ApiControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticateController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
    {
        return Ok(await _authenticationService.Login(request));
    }

    [AllowAnonymous]
    [HttpPut("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _authenticationService.VerifyEmail(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPut("verify/resend-code")]
    public async Task<IActionResult> ResendVerificationCodeForCientApp([FromBody] string email)
    {
        await _authenticationService.ResendVerification(email);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return Ok(await _authenticationService.RefreshAccessToken(request));
    }
}
