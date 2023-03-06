using Recruitment.Context;
using Recruitment.Interface;
using Microsoft.AspNetCore;
using Recruitment.Repositories;
using Roles.Controller;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();

if(builder.Environment.IsDevelopment()) {
    builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // policy.WithOrigins("/",
            //                     "127.0.0.1")
            //                     .AllowAnyHeader()
            //                     .AllowAnyMethod();
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
}

// builder.Services.AddScoped<RoleController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options => options.AllowAnyOrigin()); 

app.UseAuthorization();

app.MapControllers();

app.Run();
