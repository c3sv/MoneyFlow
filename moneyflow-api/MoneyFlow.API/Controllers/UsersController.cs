using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Users;
using MoneyFlow.Application.Users.ChangePassword;
using MoneyFlow.Application.Users.GetCurrentUser;
using MoneyFlow.Application.Users.UpdateUserProfile;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly GetCurrentUserHandler _getCurrentUserHandler;
    private readonly UpdateUserProfileHandler _updateUserProfileHandler;
    private readonly ChangePasswordHandler _changePasswordHandler;

    public UsersController(
        GetCurrentUserHandler getCurrentUserHandler,
        UpdateUserProfileHandler updateUserProfileHandler,
        ChangePasswordHandler changePasswordHandler)
    {
        _getCurrentUserHandler = getCurrentUserHandler;
        _updateUserProfileHandler = updateUserProfileHandler;
        _changePasswordHandler = changePasswordHandler;
    }

    [HttpGet("me")]
    [ProducesResponseType(
        typeof(UserProfileResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetCurrent(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetCurrentUserQuery(userId);

        var result = await _getCurrentUserHandler.HandleAsync(
            query,
            cancellationToken);

        var response = new UserProfileResponse(
            result.Id,
            result.FirstName,
            result.LastName,
            result.Email,
            result.CreatedAt);

        return Ok(response);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCurrent(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Email);

        await _updateUserProfileHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpPut("me/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmNewPassword);

        await _changePasswordHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
}