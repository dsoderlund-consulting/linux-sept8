using Microsoft.EntityFrameworkCore;
using ShoppingListApi.Data;
using ShoppingListApi.Models; // Needed for Minimal API example below if you choose that path
using System.Text; // Add this for Encoding

var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
// Attempt to read connection string parts from environment variables
// These names match the keys in your example secret (after decoding)
var dbHost = Environment.GetEnvironmentVariable("host");
var dbPort = Environment.GetEnvironmentVariable("port");
var dbName = Environment.GetEnvironmentVariable("dbname");
var dbUser = Environment.GetEnvironmentVariable("username"); // or "user" - both are in the secret
var dbPassword = Environment.GetEnvironmentVariable("password");

string? connectionString;


if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPort) &&
    !string.IsNullOrEmpty(dbName) && !string.IsNullOrEmpty(dbUser) &&
    !string.IsNullOrEmpty(dbPassword))
{
    // Construct connection string from parts
    connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};";
    Console.WriteLine("Database connection string constructed from environment variables.");
}
else
{
    // Fallback to the old method if parts are missing
    connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Warning: No database connection string configured.");
            throw new InvalidOperationException("Database connection string is not configured.");
        }
        Console.WriteLine("Database connection string loaded from DefaultConnection.");
    }
    else
    {
        Console.WriteLine("Database connection string loaded from DATABASE_CONNECTION_STRING.");
    }
}

// --- Dependency Injection ---
// Add services to the container.
builder.Services.AddDbContext<ShoppingListContext>(
    options => options.UseNpgsql(connectionString, // Use PostgreSQL
    (options) =>
        {
            options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), ["57P01"]); // PostgreSQL 57P01 ADMIN SHUTDOWN
        })
);
builder.Services.AddControllers(); // Add support for Controllers

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS Configuration ---
// Allow requests from any origin for development/demonstration.
// **IMPORTANT**: For production, restrict this to your frontend's actual hostname.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", // You can name the policy
        policy =>
        {
            policy.AllowAnyOrigin() // In production: .WithOrigins("http://your-frontend-hostname", "https://your-frontend-hostname")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// --- Application Pipeline ---
var app = builder.Build();

// Configure the HTTP request pipeline.
// Apply migrations on startup with retry logic
var maxRetries = 12; // Retry for up to 60 seconds (12 retries * 5 seconds)
var retryDelay = TimeSpan.FromSeconds(5);

for (int retry = 1; retry <= maxRetries; retry++)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Applying database migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("Database migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("No pending database migrations.");
            }
        }
        // If we reach here, migrations were successful, break out of the retry loop
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying database migrations (attempt {retry}/{maxRetries}): {ex.Message}");
        if (retry == maxRetries)
        {
            Console.WriteLine("Max retries reached. Database migrations failed.");
            throw; // Re-throw the exception after max retries
        }
        else
        {
            Console.WriteLine($"Retrying in {retryDelay.TotalSeconds} seconds...");
            Thread.Sleep(retryDelay);
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.UseHttpsRedirection(); // Often handled by ingress in Kubernetes

app.UseRouting(); // Add UseRouting before UseCors and UseAuthorization

app.UseCors("AllowAnyOrigin"); // Apply the CORS policy

app.UseAuthorization();

app.MapControllers(); // Map controller routes

app.Run(); // Start the application
