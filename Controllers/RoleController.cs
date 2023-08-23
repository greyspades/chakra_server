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
using InputFormat;
using AES;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [HttpPost("all")]
    public async Task<ActionResult> GetRoles(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobSearchDto>();

            if (payload.Value == "" && (payload.FilterType == ""))
            {
                var data = await _repo.GetRoles();

                var count = payload.Page * 10;

                var slicedList = data.Skip((int)count).Take(10);
                
                // string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // Console.WriteLine(ipAddress);

                // var searchResult = searchList.FindAll((item) => item.Name.ToLower().Contains(payload.Value.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = AEShandler.EncryptResponse(slicedList),
                    // data = slicedList,
                    count = data.Count()
                };
                // AEShandler.Decrypt(encryptedPayload, "yy7a1^.^^j_ii^c2^5^ho_@.9^d7bi^." , "h!!_2bz^(@?yyq!.");

                return Ok(response);
            } else if(payload.FilterType == "Qualification") {
                var data = await _repo.GetRoles();

                List<JobRoleModel> searchList = data.ToList();

                var searchResult = searchList.FindAll((item) => item.Qualification.ToLower().Contains(payload.Filter.ToLower()));

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = AEShandler.EncryptResponse(searchResult)
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
                    data = AEShandler.EncryptResponse(searchResult)
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
                    data = AEShandler.EncryptResponse(searchResult)
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
                    data = AEShandler.EncryptResponse(searchResult)
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
                    data = AEShandler.EncryptResponse(searchResult)
                };

                return Ok(response);
            } else {
                var data = await _repo.GetRoles();

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = AEShandler.EncryptResponse(data)
                };

                return Ok(response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            using StreamWriter outputFile = new("tokenlogs.txt", true);

            await outputFile.WriteAsync(e.Message);

            // var result = new {
            //     code = 500,
            //     message = "Unnable to process your request"
            // };

            return StatusCode(500, e.Message);
        }
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddRole(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobRoleModel>();

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
    [Authorize]
    [HttpPost("byId")]
    public async Task<ActionResult<JobRoleModel>> GetRoleById(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobRoleDto>();
            
            var data = await _repo.GetJobRoleById(payload.Id);

            var response = new
            {
                code = 200,
                message = "Successful",
                data = AEShandler.EncryptResponse(data)
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }
    [Authorize]
    [HttpGet("byUnit/{unit}")]
    public async Task<ActionResult<JobRoleModel>> GetRoleByDivision(JObject jObject)
    {
        try
        {
             var payload = jObject.ToObject<string>();

            var data = await _repo.GetJobRoleByUnit(payload);

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }
    [Authorize]
    [HttpPost("getJobRoles")]
    public async Task<ActionResult<string>> GetJobRoles(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<PaginationDto>();
            
            var data = await _repo.GetJobRoles();

            var count = payload.Page * 10;

            var slicedCandidates = data.Skip((int)count).Take((int)payload.Take);

            var response = new
            {
                code = 200,
                message = "Successful",
                count = data.Count(),
                data = AEShandler.EncryptResponse(slicedCandidates)
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    [Authorize]
    [HttpPost("JobByCode")]
    public async Task<ActionResult> GetJobByCode(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobRoleDto>();

            var data = await _repo.GetJobByCode(payload.Code);

            return Ok(AEShandler.EncryptResponse(data));
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

    [Authorize]
    [HttpPost("getJobDescription")]
    public async Task<ActionResult> GetJobDescription(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobRoleDto>();

            var data = await _repo.GetJobDescription(payload.Code);
            if (data.Any())
            {
                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = AEShandler.EncryptResponse(data)
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
    [Authorize]
    [HttpPost("search")]
    public async Task<ActionResult> SearchJob(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<JobRoleDto>();

            var data = await _repo.GetJobRoles();

            List<Job> dataList = (List<Job>)data;

            var result = dataList.FindAll((item) => item.Item.ToLower().Contains(payload.Value.ToLower()));

            return Ok(AEShandler.EncryptResponse(result));
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

    [Authorize]
    [HttpPost("status")]
    public async Task<ActionResult> ChangeJobStatus(JObject jObject) {
        try {
            
            var payload = jObject.ToObject<Job>();

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