using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.MonthlyPlans;
using MoneyFlow.Application.MonthlyPlans.AddCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.CreateMonthlyPlan;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/monthly-plans")]
public sealed class MonthlyPlansController : ControllerBase
{
    private readonly CreateMonthlyPlanHandler
        _createMonthlyPlanHandler;

    private readonly AddCategoryLimitHandler
        _addCategoryLimitHandler;

    public MonthlyPlansController(
        CreateMonthlyPlanHandler createMonthlyPlanHandler,
        AddCategoryLimitHandler addCategoryLimitHandler)
    {
        _createMonthlyPlanHandler = createMonthlyPlanHandler;
        _addCategoryLimitHandler = addCategoryLimitHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateMonthlyPlanResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateMonthlyPlanResponse>> Create(
        [FromBody] CreateMonthlyPlanRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateMonthlyPlanCommand(
            userId,
            request.Month,
            request.Year,
            request.ExpectedIncome,
            request.TargetSavings,
            request.TotalSpendingLimit,
            request.Currency);

        var monthlyPlanId =
            await _createMonthlyPlanHandler.HandleAsync(
                command,
                cancellationToken);

        var response =
            new CreateMonthlyPlanResponse(monthlyPlanId);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [HttpPost("{monthlyPlanId:long}/category-limits")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddCategoryLimit(
        long monthlyPlanId,
        [FromBody] AddCategoryLimitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new AddCategoryLimitCommand(
            userId,
            monthlyPlanId,
            request.CategoryId,
            request.LimitAmount);

        await _addCategoryLimitHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
}