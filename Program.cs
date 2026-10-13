using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Force the physical path of the current project directory as the content root
    ContentRootPath = Directory.GetCurrentDirectory()
});

// 1. Add services for API controllers (needed for ExamController)
builder.Services.AddControllers();

// ---> NEW: Add CORS policy so your HTML can fetch from this API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 2. Native OpenAPI configuration for .NET
builder.Services.AddOpenApi();

var app = builder.Build();

// ---> NEW: Enable CORS
app.UseCors();

// Configure the HTTP request processing pipeline
if (app.Environment.IsDevelopment())
{
    // Enable native OpenAPI endpoint in development mode
    app.MapOpenApi();
}

// ---> FIXED: Commented out HTTPS redirection to stop the warning!
// app.UseHttpsRedirection();

// 3. Serve default files (index.html) from project root so index.html can remain at repository root
var rootFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = rootFileProvider
});

// 4. Serve static files from the project root
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = rootFileProvider,
    RequestPath = ""
});

app.UseAuthorization();

// 5. Map API controller endpoints (enables /api/exam routes)
app.MapControllers();

app.Run();