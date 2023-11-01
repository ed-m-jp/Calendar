using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Calendar.Startup.Infra.Swagger
{
    public class RemoveDefaultResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var response in operation.Responses)
            {
                // Check if it contains the ProblemDetails default reference and remove it.
                if (response.Value.Content != null &&
                    response.Value.Content.TryGetValue("application/json", out var mediaType) &&
                    mediaType.Schema.Reference != null &&
                    mediaType.Schema.Reference.Id == "ProblemDetails")
                {
                    mediaType.Schema = null;
                }
            }
        }
    }
}
