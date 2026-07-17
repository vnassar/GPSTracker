using Microsoft.EntityFrameworkCore;
using TrackerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(Options =>
{
   Options.AddPolicy("AllowBlazor", policy =>
    policy.WithOrigins("http://localhost:5228").AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=gps_tracker.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowBlazor");
app.MapControllers();

app.Run();
