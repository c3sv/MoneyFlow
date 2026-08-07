using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.SavingsGoals;
using MoneyFlow.Application.SavingsGoals.AddSavingsGoalProgress;
using MoneyFlow.Application.SavingsGoals.CreateSavingsGoal;
using MoneyFlow.Application.SavingsGoals.DeleteSavingsGoal;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoalById;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoals;
using MoneyFlow.Application.SavingsGoals.UpdateSavingsGoal;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/savings-goals")]
public sealed class SavingsGoalsController : ControllerBase
{
    private readonly CreateSavingsGoalHandler _createSavingsGoalHandler;
    private readonly GetSavingsGoalsHandler _getSavingsGoalsHandler;
    private readonly GetSavingsGoalByIdHandler _getSavingsGoalByIdHandler;
    private readonly UpdateSavingsGoalHandler _updateSavingsGoalHandler;
    private readonly AddSavingsGoalProgressHandler _addSavingsGoalProgressHandler;
    private readonly DeleteSavingsGoalHandler _deleteSavingsGoalHandler;

    public SavingsGoalsController(
        CreateSavingsGoalHandler createSavingsGoalHandler,
        GetSavingsGoalsHandler getSavingsGoalsHandler,
        GetSavingsGoalByIdHandler getSavingsGoalByIdHandler,
        UpdateSavingsGoalHandler updateSavingsGoalHandler,
        AddSavingsGoalProgressHandler addSavingsGoalProgressHandler,
        DeleteSavingsGoalHandler deleteSavingsGoalHandler)
    {
        _createSavingsGoalHandler = createSavingsGoalHandler;
        _getSavingsGoalsHandler = getSavingsGoalsHandler;
        _getSavingsGoalByIdHandler = getSavingsGoalByIdHandler;
        _updateSavingsGoalHandler = updateSavingsGoalHandler;
        _addSavingsGoalProgressHandler = addSavingsGoalProgressHandler;
        _deleteSavingsGoalHandler = deleteSavingsGoalHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateSavingsGoalResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateSavingsGoalResponse>> Create(
        [FromBody] CreateSavingsGoalRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateSavingsGoalCommand(
            userId,
            request.Title,
            request.TargetAmount,
            request.Deadline);

        var savingsGoalId =
            await _createSavingsGoalHandler.HandleAsync(
                command,
                cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new CreateSavingsGoalResponse(savingsGoalId));
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<SavingsGoalResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<SavingsGoalResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetSavingsGoalsQuery(userId);

        var results = await _getSavingsGoalsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = results
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{savingsGoalId:long}")]
    [ProducesResponseType(
        typeof(SavingsGoalResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SavingsGoalResponse>> GetById(
        long savingsGoalId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetSavingsGoalByIdQuery(
            userId,
            savingsGoalId);

        var result = await _getSavingsGoalByIdHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(ToResponse(result));
    }

    [HttpPut("{savingsGoalId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long savingsGoalId,
        [FromBody] UpdateSavingsGoalRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateSavingsGoalCommand(
            userId,
            savingsGoalId,
            request.Title,
            request.TargetAmount,
            request.Deadline);

        await _updateSavingsGoalHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{savingsGoalId:long}/progress")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddProgress(
        long savingsGoalId,
        [FromBody] AddSavingsGoalProgressRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new AddSavingsGoalProgressCommand(
            userId,
            savingsGoalId,
            request.Amount);

        await _addSavingsGoalProgressHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{savingsGoalId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long savingsGoalId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new DeleteSavingsGoalCommand(
            userId,
            savingsGoalId);

        await _deleteSavingsGoalHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    private static SavingsGoalResponse ToResponse(
        SavingsGoalResult savingsGoal)
    {
        return new SavingsGoalResponse(
            savingsGoal.Id,
            savingsGoal.Title,
            savingsGoal.TargetAmount,
            savingsGoal.CurrentAmount,
            savingsGoal.Deadline);
    }
}