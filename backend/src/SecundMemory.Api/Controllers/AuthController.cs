using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Commands.Login;
using SecundMemory.Application.Features.Auth.Commands.RefreshToken;
using SecundMemory.Application.Features.Auth.Commands.Register;
using SecundMemory.Application.Features.Auth.Dtos;

namespace SecundMemory.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IDispatcher dispatcher, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Personal deployment: set Auth__AllowRegistration=false once your own account exists.
        if (!configuration.GetValue("Auth:AllowRegistration", true))
            return StatusCode(StatusCodes.Status403Forbidden, new { title = "Registration is disabled on this server." });

        return Ok(await dispatcher.Send(command, cancellationToken));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginCommand command, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(command, cancellationToken));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh(RefreshTokenCommand command, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(command, cancellationToken));
}
