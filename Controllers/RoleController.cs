using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roles.Models;
using JobRole.Interface;
using Recruitment.Context;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using System.Diagnostics;
// using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using JobRole.Interface;

namespace Roles.Controller;

[Route("roles/[Controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly IJobRoleRepository _repo;
    public RoleController(IConfiguration config, ILogger<dynamic> logger, IJobRoleRepository repo)
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

        var data = await _repo.GetRoles();

        var response = new
        {
            code = 200,
            message = "Successful",
            data
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> AddRole(JobRoleModel payload)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            payload.Id = guid.ToString();

            var duplicate = await _repo.GetJobByCode(payload.Code);


            if(!duplicate.Any()) {
                await _repo.AddJobRole(payload);
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

    [HttpGet("byId/{id}")]
    public async Task<ActionResult<JobRoleModel>> GetRoleById(string id)
    {
        try
        {
            var data = await _repo.GetJobRoleById(id);

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
    public async Task<ActionResult<JobRoleModel>> GetRoleByDivision(string unit)
    {
        try
        {
            var data = await _repo.GetJobRoleByUnit(unit);

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
            if(data.Any()) {
                var response = new {
                    code = 200,
                    message = "Successful",
                    data
                };

                return Ok(response);
            }
            else {
                var response = new {
                    code = 500,
                    message = "couldnt"
                };

                return StatusCode(500, response);
            }
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
            List<Job> dataList = (List<Job>)data;

            var result = dataList.FindAll((item) => item.Item.ToLower().Contains(value.ToLower()));

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