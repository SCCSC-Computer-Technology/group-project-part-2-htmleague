using HTMLeagueGroupProjectAPI.Repositories;
using Microsoft.AspNetCore.Mvc.Formatters;
using static System.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using HTMLeagueGroupProjectAPI.Data;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using System.Text.Json.Serialization;
using SportStatsWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup data ditectory
AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(Directory.GetCurrentDirectory(), "Data"));

// Register DbContext
builder.Services.AddDbContext<CsgoContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BGSports")
    ));
builder.Services.AddDbContext<NflContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BGSports")
    ));
builder.Services.AddDbContext<NbaContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BGSports")
    ));
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BGSports")
    ));

builder.Services.AddScoped<ICRUDCsgo, CsgoRepo>();
builder.Services.AddScoped<ICRUDNba, NbaRepo>();
builder.Services.AddScoped<ICRUDNfl, NflRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
