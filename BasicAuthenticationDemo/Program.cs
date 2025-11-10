using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuth>("BasicAuthentication", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Developer", policy => policy.RequireRole("Developer"));
});



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "Public Endpoint (no auth)");
app.MapGet("/secure-admin", [Authorize("Admin")] (ClaimsPrincipal user) =>
{
    var username = user?.Identity?.Name;
    var roles = user?.Claims.Where(d => d.Type == ClaimTypes.Role).Select(c => c.Value);
    var srole = string.Join(",", roles!);
    return $"Hoş geldin {username} , role: {srole}";

}).WithName("secure-admin");

app.MapGet("/secure-devleoper", [Authorize("Developer")] (ClaimsPrincipal user) =>
{
    var username = user?.Identity?.Name;
    var roles = user?.Claims.Where(d => d.Type == ClaimTypes.Role).Select(c => c.Value);
    var srole = string.Join(",", roles!);
    return Results.Ok($"Hoş geldin {username} , role: {srole}");

}).WithName("secure-devleoper");



/*app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");*/

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
