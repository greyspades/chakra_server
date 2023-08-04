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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHostedService<TimedHostedService>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();

// builder.Services.Configure<ResumeDbSettings>(
//     builder.Configuration.GetSection("ResumeDatabase"));

var Configuration = builder.Configuration;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.Cookie.Name = "cookieAuth";
        options.Cookie.SameSite = SameSiteMode.None; //TODO is this important?
        options.Cookie.HttpOnly = false;
        options.SlidingExpiration = true;
        options.LoginPath = "/api/Candidate/signin";
        options.LogoutPath = "/api/User/logout";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(hostName => true);
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


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(hostName => true);
        });
});
}
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    // HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.None,
};

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseRateLimiter();
    app.UseCookiePolicy();
    app.UseAuthorization();
    app.UseAuthentication();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}
if (app.Environment.IsProduction())
{
    app.UseRateLimiter();
    app.UseCookiePolicy();
    app.UseAuthorization();
    app.UseAuthentication();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Hello {GetTicks()}"))
                           .RequireRateLimiting("fixed");

app.UseCors();

app.UseCookiePolicy();

app.UseAuthorization();

app.UseAuthentication();


app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    },

});

app.MapDefaultControllerRoute();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
