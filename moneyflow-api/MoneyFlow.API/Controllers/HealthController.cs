using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoneyFlow.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(HealthResponse),
        StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        var response = new HealthResponse(
            "Healthy",
            "MoneyFlow.API");

        return Ok(response);
    }
}

public sealed record HealthResponse(
    string Status,
    string Service);