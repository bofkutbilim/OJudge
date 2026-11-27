using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OJudge.Services;

public sealed class BearerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any()) return;
        var bearer = new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, context.Document);
        operation.Security = [new OpenApiSecurityRequirement { [bearer] = [] }];
    }
}
