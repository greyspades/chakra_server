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
// using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using JobRole.Interface;
using System.Text.Json;

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
    public async Task<ActionResult> GetRoles(JobSearchDto payload)
    {
        try
        {
            if (payload.Value == "" && (payload.FilterType == ""))
            {
                var data = await _repo.GetRoles();

                var count = payload.Page * 10;

                var slicedList = data.Skip((int)count).Take(10);

                // var searchResult = searchList.FindAll((item) => item.Name.ToLower().Contains(payload.Value.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = slicedList,
                    count = data.Count()
                };

                return Ok(response);
            } else if(payload.FilterType == "Qualification") {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList();

                var searchResult = searchList.FindAll((item) => item.Qualification.ToLower().Contains(payload.Filter.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = searchResult
                };

                return Ok(response);
            } else if(payload.FilterType == "JobType") {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList();

                var searchResult = searchList.FindAll((item) => item.JobType.ToLower().Contains(payload.Filter.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = searchResult
                };

                return Ok(response);
            } else if(payload.FilterType == "location") {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList();

                // var searchResult = searchList.FindAll((item) => item.Location.ToLower() == "bayelsa");
                var searchResult = searchList.FindAll((item) => item.Location.ToLower().Contains(payload.Filter.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = searchResult
                };

                return Ok(response);
            }
            else if(payload.FilterType == "skill") {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList();

                var searchResult = searchList.FindAll((item) => JsonSerializer.Deserialize<List<string>>(item.Skills).Contains(payload.Filter.ToLower().Trim()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = searchResult
                };

                return Ok(response);
            }
            else if(payload.Value != "")
            {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList<JobRoleModel>();

                var searchResult = searchList.FindAll((item) => item.Name.ToLower().Contains(payload.Value.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = searchResult
                };

                return Ok(response);
            } else {
                var data = await _repo.GetRoles();

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data
                };

                return Ok(response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            using StreamWriter outputFile = new("tokenlogs.txt", true);

            await outputFile.WriteAsync(e.Message);

            var result = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, result);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddRole(JobRoleModel payload)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var duplicate = await _repo.GetJobByCode(payload.Code);


            if (!duplicate.Any())
            {
                payload.Id = guid.ToString();
                await _repo.AddJobRole(payload);
            }

            else
            {
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

            using StreamWriter outputFile = new("tokenlogs.txt", true);

            await outputFile.WriteAsync(e.Message);

            var result = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, result);
        }
    }

    [HttpPost("byId")]
    public async Task<ActionResult<JobRoleModel>> GetRoleById(JobRoleDto payload)
    {
        try
        {
            var data = await _repo.GetJobRoleById(payload.Id);

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
    public async Task<ActionResult<string>> GetJobRoles(PaginationDto payload)
    {
        try
        {
            var data = await _repo.GetJobRoles();

            var count = payload.Page * 10;

            var slicedCandidates = data.Skip((int)count).Take((int)payload.Take);

            var response = new
            {
                code = 200,
                message = "Successful",
                count = data.Count(),
                data = slicedCandidates
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("JobByCode")]
    public async Task<ActionResult> GetJobByCode(JobRoleDto payload)
    {
        try
        {
            var data = await _repo.GetJobByCode(payload.Code);

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            var response = new
            {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }


    [HttpPost("getJobDescription")]
    public async Task<ActionResult> GetJobDescription(JobRoleDto payload)
    {
        try
        {
            var data = await _repo.GetJobDescription(payload.Code);
            if (data.Any())
            {
                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data
                };

                return Ok(response);
            }
            else
            {
                var response = new
                {
                    code = 500,
                    message = "couldnt"
                };

                return StatusCode(500, response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            var response = new
            {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult> SearchJob(JobRoleDto payload)
    {
        try
        {

            var data = await _repo.GetJobRoles();
            List<Job> dataList = (List<Job>)data;

            var result = dataList.FindAll((item) => item.Item.ToLower().Contains(payload.Value.ToLower()));

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            var response = new
            {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }
    [HttpPost("status")]
    public async Task<ActionResult> ChangeJobStatus(Job payload) {
        try {
            await _repo.ChangeJobStatus(payload);

            var response = new {
                code = 200,
                message = "Request processed successfully"
            };

            return Ok(response);
        } catch(Exception e) {
            Console.WriteLine(e.Message);

            var response = new {
                code = 500,
                message = "Unnable to complete your request"
            };
            return StatusCode(500, response);
        }
    }
}