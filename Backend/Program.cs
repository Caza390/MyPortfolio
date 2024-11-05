using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:5173", "http://192.168.1.90:5173") // Your frontend's local URL
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials(); // Allow cookies, if needed
        });
});

// Configure MySQL connection using configuration from appsettings.json and environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Specify the server URL (Railway will use its own port in production, but this is useful for local testing)
app.Urls.Add("http://0.0.0.0:5176");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the CORS policy
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StoredImages")),
    RequestPath = "/StoredImages"
});

app.UseCors("AllowFrontendDev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
