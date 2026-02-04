using FluentValidation;
using GoalTracker.Data;
using GoalTracker.DTOs.GoalDTOs.Validators;
using GoalTracker.Profiles;
using GoalTracker.Repositories;
using GoalTracker.Repositories.Interfaces;
using GoalTracker.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IGoalTaskRepository, GoalTaskRepository>();
builder.Services.AddScoped<GoalService>();
builder.Services.AddScoped<GoalTaskService>();
builder.Services.AddValidatorsFromAssemblyContaining<GoalCreateValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
