using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var mySecretKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_KEY");

// 1. Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Controllers
builder.Services.AddControllers();

// 3. Azure Blob Storage
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------------------------------
// IMPORTANT : on construit l'application AVANT d'utiliser "app"
// ------------------------------------------------------------
var app = builder.Build();

// 5. Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Elearning Smart Learn Quiz APP API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();