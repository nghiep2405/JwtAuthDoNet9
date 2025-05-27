using JwtAuthDoNet9.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JwtAuthDoNet9.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));
builder.Services.AddScoped<IAuthService, AuthService>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),

        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                       System.Text.Encoding.UTF8.GetBytes(
                           builder.Configuration.GetValue<string>("AppSettings:Token") ??
                           "hellooooooooooooooooooooooooooooooooooooooooooooooooooooo1235555555555566666666666666666777777777777777777777777777777777777999999999999999999999911111111111110000000000000000000"
                       )
                   )
    };
});


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
