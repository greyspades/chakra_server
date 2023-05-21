using Recruitment.Context;
using Candidate.Interface;
using JobRole.Interface;
using Microsoft.AspNetCore;
using Candidate.Repository;
using Microsoft.Extensions.FileProviders;
using Resume.Models;
using Cron.Handler;
using Quartz;
using Microsoft.AspNetCore.Authentication.Cookies;
using Jobrole.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();

builder.Services.Configure<ResumeDbSettings>(
    builder.Configuration.GetSection("ResumeDatabase"));

var Configuration = builder.Configuration;

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("Contract");
    q.AddJob<LasmService>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("renewal")
        // .WithCronSchedule("* */5 * * * ?"))
        .WithSimpleSchedule(a => a.WithIntervalInMinutes(28).RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// builder.Services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
//         .EnableTokenAcquisitionToCallDownstreamApi()
//         .AddMicrosoftGraph("https://graph.microsoft.com/beta", scopes)
//         .AddInMemoryTokenCaches();

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
    app.UseCookiePolicy();
    app.UseAuthorization();
    app.UseAuthentication();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}
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
