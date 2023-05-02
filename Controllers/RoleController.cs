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
    private readonly ICandidateRepository _repo;
    public RoleController(IConfiguration config, ILogger<dynamic> logger, ICandidateRepository repo)
    {
        this._config = config;
        this._logger = logger;
        this._repo = repo;
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

            var duplicate = await connection.QueryAsync<RoleModel>("SELECT * FROM roles WHERE code = @Code", payload);

            if(!duplicate.Any()) {
                var data = await connection.ExecuteAsync("INSERT into Roles(id, name, description, experience, deadline, unit, code, status) VALUES (@Id, @Name, @Description, @Experience, @Deadline, @Unit, @Code, @Status)", payload);
            }

            else {
                return Ok(new
            {
                code = 501,
                message = "This job is already active",
            });
            }

            var response = new
            {
                code = 200,
                message = "Successfully added new Role",
            };


            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(value: e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("edit")]
    public async Task<ActionResult<RoleModel>> UpdateRole(RoleModel payload) {
        try {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var data = await connection.ExecuteAsync("UPDATE roles SET id=@Id, name=@Name, unit=@Unit, description=@Description, experience=@Experience, deadline=@Deadline WHERE id = @Id", payload);

            var response = new
            {
                code = 200,
                message = "Successfully Updated Role",
                data
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(value: e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("byId/{id}")]
    public async Task<ActionResult<RoleModel>> GetRoleById(string id)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var data = await connection.QueryAsync("SELECT * FROM roles where id = @Id", new { Id = id });

            var response = new {
                code = 200,
                message = "Successful",
                data
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("byUnit/{unit}")]
    public async Task<ActionResult<RoleModel>> GetRoleByDivision(string unit)
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

    [HttpGet("jobs/{page}/{take}")]
    public async Task<ActionResult<string>> GetJobs(int page, int take) {
        try {
            var data = await _repo.GetJobRoles();

            // var take = 10;

            var count = page * 10;
            
            var slicedCandidates = data.Skip(count).Take(take);

            var response = new {
                code = 200,
                message = "Successful",
                count = data.Count(),
                data = slicedCandidates
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult> GetJobByCode(string code) {
        try {
            var data = await _repo.GetJobByCode(code);

            return Ok(data);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);

            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    
    [HttpGet("description/{code}")]
    public async Task<ActionResult> GetJobDescription(string code) {
        try {
            var data = await _repo.GetJobDescription(code);

            return Ok(data);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);

            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }
    
    [HttpGet("search/{value}")]
    public async Task<ActionResult> SearchJob(string value) {
        try {
            
            var data = await _repo.GetJobRoles();

            var dataList = data.ToList();

            var result = dataList.Find((item) => item.Item.Contains(value));

            return Ok(result);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);

            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

}