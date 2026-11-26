using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GameBackendAPI.OpenAPI
{
    public class OpenApiOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var metaData = context.Description.ActionDescriptor.EndpointMetadata;

            if (!metaData.OfType<IAuthorizeData>().Any() || metaData.OfType<IAllowAnonymous>().Any())
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];
            operation.Security.Add(new() { [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = [] });

            return Task.CompletedTask;
        }
    }
}
