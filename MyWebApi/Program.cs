using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;
using Repositories;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//IOC container - DEPENDENCY INJECTION 
builder.Services.AddControllersWithViews(); // Ezzel biztosítjuk a TempData és Views támogatását
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

public partial class Program { } //make the auto-generated Program accessible programmatically