using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger en Production
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Elearning Smart Learn Quiz APP API V1");
    options.RoutePrefix = string.Empty;
});

// Désactiver HTTPS redirection en Production
// app.UseHttpsRedirection();

app.UseAuthorization();

// Fichiers statiques (PDF)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

// API
app.MapControllers();

// Rendre l’API accessible depuis l’extérieur
app.Urls.Add("http://0.0.0.0:5000");

app.Run();
