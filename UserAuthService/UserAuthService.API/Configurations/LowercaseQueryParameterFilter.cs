using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UserAuthService.API.Configurations
{
    public class LowercaseQueryParameterFilter : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                parameter.Name =
                    char.ToLowerInvariant(parameter.Name[0]) +
                    parameter.Name.Substring(1);
            }
        }
    }
}
