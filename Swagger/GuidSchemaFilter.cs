using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class GuidSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Guid) || context.Type == typeof(Guid?))
        {
            schema.Type = "string";
            schema.Format = null; // enlève "uuid"
        }
    }
}
