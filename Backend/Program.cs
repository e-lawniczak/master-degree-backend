using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System;
using System.Configuration;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var congfig = builder.Configuration;
// Add services to the container.
var itchIo = "_itchIoOrigins";
var open = "_openOrigins";


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swg =>
{
    swg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    swg.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
    swg.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

});
builder.Services.AddCors(options =>
{
    options.AddPolicy(itchIo, builder =>
    {
        builder
        //.WithOrigins(
        //    "https://html-classic.itch.zone/",
        //    "https://html.itch.zone",
        //    "https://*.itch.zone",
        //    "https://oxygen-enjoyer.itch.io"
        //    )
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy( policy => 
//    policy
//    //.WithOrigins("https://html-classic.itch.zone/")
//    .AllowAnyOrigin()
//    .AllowAnyMethod()
//    .AllowAnyHeader()
//    );
//});

builder.Services.AddDbContext<DbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnectionString")
    ));

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = System.Configuration.ConfigurationManager.AppSettings["Issuer"],
        ValidAudience = System.Configuration.ConfigurationManager.AppSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings["Key"])),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,

    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(itchIo);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
