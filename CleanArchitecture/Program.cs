using Application.Interfaces.IRepo;
using Application.Interfaces.IServices;
using Application.UseCases.Students;
using CleanArchitecture.middleware;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IExceptionMapper, DefaultExceptionMapper>();
builder.Services.AddScoped<StudentService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
        options.RoutePrefix = "swagger";
    });
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
        options.RoutePrefix = "redoc"; // http://localhost:xxxx/redoc
        options.DocumentTitle = "My API Docs";
    });

    app.MapScalarApiReference();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
