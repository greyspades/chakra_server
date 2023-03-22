using Recruitment.Context;
using Recruitment.Interface;
using Microsoft.AspNetCore;
using Recruitment.Repositories;
using Microsoft.Extensions.FileProviders;
using Resume.Models;

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
}

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
