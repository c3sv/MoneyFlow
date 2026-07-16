using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Contracts.Authentication;
using MoneyFlow.Application.Authentication.Login;
using MoneyFlow.Application.Authentication.Register;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;

    public AuthenticationController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(
        typeof(AuthenticationResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await _registerHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new AuthenticationResponse(
            result.UserId,
            result.FirstName,
            result.LastName,
            result.Email,
            result.Token);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(AuthenticationResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await _loginHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new AuthenticationResponse(
            result.UserId,
            result.FirstName,
            result.LastName,
            result.Email,
            result.Token);

        return Ok(response);
    }
}