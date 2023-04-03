using Recruitment.Context;
using Recruitment.Interface;
using Microsoft.AspNetCore;
using Recruitment.Repositories;
using Microsoft.Extensions.FileProviders;
using Resume.Models;
using Cron.Handler;
using Quartz;
using MailConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.Configure<ResumeDbSettings>(
    builder.Configuration.GetSection("ResumeDatabase"));

// builder.Services.AddQuartz(q =>
// {
//     q.UseMicrosoftDependencyInjectionJobFactory();
//     var jobKey = new JobKey("Contract-renewal");
//     q.AddJob<LasmService>(opts => opts.WithIdentity(jobKey));

//     q.AddTrigger(opts => opts
//         .ForJob(jobKey)
//         .WithIdentity("contract-trigger")
//         .WithCronSchedule("*/25 * * * * ?"));

// });

// builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

if(builder.Environment.IsDevelopment()) {
    builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}

app.MapControllers();

app.UseCors(options => options.AllowAnyOrigin()); 

app.UseStaticFiles(new StaticFileOptions()
        {
            ServeUnknownFileTypes = true,
            OnPrepareResponse = ctx => {
            ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                
                // ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                // ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", 
                //   "Origin, X-Requested-With, Content-Type, Accept");
            },

        });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
