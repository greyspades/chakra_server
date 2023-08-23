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
using Middleware.NoSniff;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

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

// builder.Services.AddMemoryCache();
//     builder.Services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
//     builder.Services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

//     builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//* configures authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CustomAuthenticationOptions.AuthenticationScheme;
        options.DefaultChallengeScheme = CustomAuthenticationOptions.AuthenticationScheme;
    })
    .AddCustomAuthentication();

//* hsts configuration
// builder.Services.AddHsts(options =>
//             {
//                 options.MaxAge = TimeSpan.FromDays(365); // Set the max-age value (1 year in this example)
//                 options.IncludeSubDomains = true; // Include subdomains
//             });


//* configures the cors setup
builder.Services.AddCors(options =>
{
     options.AddPolicy("ReactPolicy", builder =>
        {
            builder
                   .WithOrigins(Configuration.GetValue<string>("Production:Url"))
                //    .AllowAnyOrigin()
                   .WithMethods("GET", "POST")
                   .WithHeaders("Content-Type","Access-Control-Allow-Origin", "Auth");
        });
});

//* configures the ratelimiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Request.Headers.Host.ToString(), partition =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                AutoReplenishment = true,
                QueueLimit = 5,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                Window = TimeSpan.FromSeconds(10)
            });
    });
    //* custom response if there are too many requests
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later... ", cancellationToken: token);
    };
});

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseMiddleware<NoSniffMiddleware>();

app.UseCors("ReactPolicy");

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}
if (app.Environment.IsProduction())
{ 
    app.UseHsts();
}

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

//* static file sharing
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
