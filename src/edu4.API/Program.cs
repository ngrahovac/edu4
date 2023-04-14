using System.Security.Claims;
using edu4.API.DI;
using edu4.API.Middleware;
using edu4.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddIAMServices();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{config["Auth0:Domain"]}/";
        options.Audience = config["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization(configure => configure.AddPolicy(
        "NonContributor",
        policy => policy.RequireAssertion(context =>
        !context.User.HasClaim(c => c.Type == "permissions") ||
        context.User.Claims.FirstOrDefault(c => c.Type == "permissions")?.Value == string.Empty)
        ));

builder.Services.AddAuthorization(configure => configure.AddPolicy(
        "Contributor",
        policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy.RequireAssertion(_ => true);
            }
            else
            {
                policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "permissions") &&
                        context.User.Claims.First(c => c.Type == "permissions").Value.Contains("contribute"));
            }
        }));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IAccountIdExtractionService, TestAccountIdExtractionService>();
}
else
{
    builder.Services.AddSingleton<IAccountIdExtractionService, AccountIdExtractionService>();
}

builder.Services.AddCors(builder =>
    builder.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyOrigin();
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
