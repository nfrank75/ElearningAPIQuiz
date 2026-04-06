var builder = WebApplication.CreateBuilder(args);

// 1. Gardez vos services actuels
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Le moteur de .NET 9

var app = builder.Build();

// 2. Configurez le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Active la génération du document JSON (v1/openapi.json)
    app.MapOpenApi();

    // ACTIVE L'INTERFACE VISUELLE (SWAGGER UI)
    // On pointe sur le fichier JSON généré par MapOpenApi
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ElearningAPI v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();