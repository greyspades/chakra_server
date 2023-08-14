using Recruitment.Context;
using Candidate.Interface;
using JobRole.Interface;
using Microsoft.AspNetCore;
using Candidate.Repository;
using Microsoft.Extensions.FileProviders;
using Resume.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Jobrole.Repository;
using TimedBackgroundTasks;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using EncryptMiddleware;
using InputFormat;
using AuthHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(
    (options) => options.InputFormatters.Insert(0, new XInputFormatter())
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHostedService<TimedHostedService>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();


var Configuration = builder.Configuration;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CustomAuthenticationOptions.AuthenticationScheme;
        options.DefaultChallengeScheme = CustomAuthenticationOptions.AuthenticationScheme;
    })
    .AddCustomAuthentication();

builder.Services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365); // Set the max-age value (1 year in this example)
                options.IncludeSubDomains = true; // Include subdomains
            });


builder.Services.AddCors(options =>
{
     options.AddPolicy("ReactPolicy", builder =>
        {
            builder
                //    .WithOrigins("https://10.0.1.46:8443", "http://localhost:3000")
                   .AllowAnyOrigin()
                   .WithMethods("GET", "POST")
                   .WithHeaders("Content-Type","Access-Control-Allow-Origin", "Auth");
        });
});

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

var app = builder.Build();

app.UseCors("ReactPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
        app.UseAuthentication();
     app.UseAuthorization();
    // app.UseRateLimiter();
    // app.UseCookiePolicy();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}
if (app.Environment.IsProduction())
{ 
    app.UseHsts();
    app.UseAuthentication();
    app.UseAuthorization();
    // app.UseRateLimiter();
    // app.UseCookiePolicy();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}

// static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

// app.MapGet("/", () => Results.Ok($"Hello {GetTicks()}"))
//                            .RequireRateLimiting("fixed");

// app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    },
});

// app.MapDefaultControllerRoute();

// app.UseHttpsRedirection();

// app.MapControllers();

app.Run();
