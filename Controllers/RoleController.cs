using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roles.Models;
using Recruitment.Interface;
using Recruitment.Context;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using System.Diagnostics;

namespace Roles.Controller;

[Route("roles/[Controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    public RoleController(IConfiguration config, ILogger<dynamic> logger)
    {
        this._config = config;
        this._logger = logger;
    }

    // [EnableCors("Policy1")]
    [HttpGet]
    public async Task<ActionResult> GetRoles()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<RoleModel>("SELECT * FROM Roles");

        var response = new
        {
            code = 200,
            message = "Successful",
            data
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> AddRole(RoleModel payload)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            payload.Id = guid.ToString();
            // Debug.WriteLine(payload);
            // Console.WriteLine(payload);
            _logger.LogInformation(payload.Name);
            var data = await connection.ExecuteAsync("INSERT into Roles(id, name, description, experience, deadline, unit) VALUES (@Id, @Name, @Description, @Experience, @Deadline, @Unit)", payload);

            var response = new
            {
                code = 200,
                message = "Successful",
                data
            };


            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(value: e.Message);

            return StatusCode(500, e.Message);
        }
    }

    // [HttpPost]
    // public async Task<ActionResult> UpdateRole(RoleModel payload) {
    //     try{
    //         using var connection = new SqlConnection(_config.GetConnectionString("DEfaultConnection"));

    //         var data = await connection.ExecuteAsync("",payload);

    //         return Ok(data);
    //     }
    //     catch(Exception e) {

    //         Console.WriteLine(e.Message);

    //         return StatusCode(500, e.Message);
    //     }
    // }

    [HttpGet("byId/{id}")]
    public async Task<ActionResult<RoleModel>> GetRoleById(String id)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var data = await connection.QueryAsync("SELECT * FROM roles where id = @Id", new { Id = id });

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("byUnit/{unit}")]
    public async Task<ActionResult<RoleModel>> GetRoleByDivision(String unit)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var data = await connection.QueryAsync("SELECT * FROM roles where unit = @Unit", new { Unit = unit });

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }
}