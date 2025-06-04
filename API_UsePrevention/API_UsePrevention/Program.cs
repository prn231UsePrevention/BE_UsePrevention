using System.Text;
using API_UsePrevention.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Repositories;
using Repository.UWO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


//config db
builder.Services.AddDatabaseServices(builder.Configuration);


//Register repository;;
builder.Services.AddRepositoryServices();

//Register service;
builder.Services.AddAppServices();

//config JWT
builder.Services.AddJwtAuthentication(builder.Configuration);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
