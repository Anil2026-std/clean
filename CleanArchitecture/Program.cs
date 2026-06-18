using Application.Interfaces.IRepo;
using Application.Interfaces.IServices;
using Application.ServiceDto;
using Application.Services.AuthServices;
using Application.Services.UserServices;
using Application.UseCases.Students;
using Application.Validator;
using CleanArchitecture.middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.auth;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IExceptionMapper, DefaultExceptionMapper>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();


builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddFluentValidationAutoValidation(); this comment beacuse we used the mustasync 
builder.Services.AddFluentValidationClientsideAdapters();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
