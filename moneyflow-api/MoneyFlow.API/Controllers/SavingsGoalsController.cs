using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.SavingsGoals;
using MoneyFlow.Application.SavingsGoals.AddSavingsGoalProgress;
using MoneyFlow.Application.SavingsGoals.CreateSavingsGoal;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoals;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/savings-goals")]
public sealed class SavingsGoalsController : ControllerBase
{
    private readonly CreateSavingsGoalHandler
        _createSavingsGoalHandler;

    private readonly AddSavingsGoalProgressHandler
        _addSavingsGoalProgressHandler;

    private readonly GetSavingsGoalsHandler _getSavingsGoalsHandler;
    public SavingsGoalsController(
        CreateSavingsGoalHandler createSavingsGoalHandler,
        AddSavingsGoalProgressHandler addSavingsGoalProgressHandler,
        GetSavingsGoalsHandler getSavingsGoalsHandler)
    {
        _createSavingsGoalHandler = createSavingsGoalHandler;
        _addSavingsGoalProgressHandler =
            addSavingsGoalProgressHandler;
        _getSavingsGoalsHandler = getSavingsGoalsHandler;
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
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
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

        var response =
            new CreateSavingsGoalResponse(savingsGoalId);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [HttpPatch("{savingsGoalId:long}/progress")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
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
            .Select(goal => new SavingsGoalResponse(
                goal.Id,
                goal.Title,
                goal.TargetAmount,
                goal.CurrentAmount,
                goal.Deadline))
            .ToList();

        return Ok(response);
    }
}