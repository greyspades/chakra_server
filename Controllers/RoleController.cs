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
    [HttpPost("all")]
    public async Task<ActionResult> GetRoles(JobRoleDto payload)
    {
        var data = await _repo.GetRoles();

            // var data = await _repo.GetJobRoles();
            List<JobRoleModel> dataList = (List<JobRoleModel>)data;

            data = dataList.FindAll((item) => item.Name.ToLower().Contains(payload.Value.ToLower()));


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

            var duplicate = await _repo.GetJobByCode(payload.Code);

            if(!duplicate.Any()) {
                payload.Id = guid.ToString();
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

    [HttpPost("byId")]
    public async Task<ActionResult<JobRoleModel>> GetRoleById(JobRoleDto payload)
    {
        try
        {
            var data = await _repo.GetJobRoleById(payload.Id);

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

    [HttpPost("getJobRoles")]
    public async Task<ActionResult<string>> GetJobRoles(PaginationDto payload) {
        try {
            var data = await _repo.GetJobRoles();

            var count = payload.Page * 10;
            
            var slicedCandidates = data.Skip((int)count).Take((int)payload.Take);

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

    [HttpPost("JobByCode")]
    public async Task<ActionResult> GetJobByCode(JobRoleDto payload) {
        try {
            var data = await _repo.GetJobByCode(payload.Code);

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

    
    [HttpPost("getJobDescription")]
    public async Task<ActionResult> GetJobDescription(JobRoleDto payload) {
        try {
            var data = await _repo.GetJobDescription(payload.Code);
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
    
    [HttpPost("search")]
    public async Task<ActionResult> SearchJob(JobRoleDto payload) {
        try {
            
            var data = await _repo.GetJobRoles();
            List<Job> dataList = (List<Job>)data;

            var result = dataList.FindAll((item) => item.Item.ToLower().Contains(payload.Value.ToLower()));

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