using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MoneyFlow.API.OpenApi;

public sealed class AuthOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var endpointMetadata =
            context.Description.ActionDescriptor.EndpointMetadata;

        var requiresAuthorization = endpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any();

        var allowsAnonymous = endpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (!requiresAuthorization || allowsAnonymous)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        context.Document)
                ] = []
            });

        return Task.CompletedTask;
    }
}