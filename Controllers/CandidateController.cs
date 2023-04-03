using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Recruitment.Interface;
using Microsoft.AspNetCore.Cors;
using System.Security;
using Microsoft.AspNetCore.StaticFiles;
using Resume.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using AES;
using CredentialsHandler;

namespace Candidate.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly ICandidateRepository _repo;
    public IWebHostEnvironment _env;
    private readonly IMongoCollection<ResumeModel> _collection;
    private readonly IMongoDatabase _mongoDb;
    private static readonly HttpClient sharedClient = new();
    public CandidateController(IWebHostEnvironment environment, ICandidateRepository repo, IOptions<ResumeDbSettings> dbSettings)
    {
        this._repo = repo;
        this._env = environment;
        var mongoClient = new MongoClient(
            dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<ResumeModel>(
            dbSettings.Value.CollectionName);
        _mongoDb = mongoDatabase;
    }

    [HttpGet]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidates()
    {
        try
        {
            var data = await _repo.GetCandidates();

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateById(string id)
    {
        try
        {
            var data = await _repo.GetCandidateById(id);

            var sample = new
            {
                code = 200,
                message = "Successful",
                data
            };

            return Ok(sample);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [DisableCors]
    [HttpPost]
    public async Task<ActionResult<List<CandidateModel>>> CreateCandidate([FromForm] CandidateModel payload)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var dateDifference = (DateTime.Now.Date - payload?.Dob.Value)?.TotalDays;

                var mail = await _repo.CheckEmail(payload!);

                if (
                    dateDifference > 6570 &&
                !mail.Any())
                {
                    var uuid = guid;
                    if (payload.Cv != null)
                    {
                        var fileData = await _repo.ParseCvAsync(payload.Cv, uuid);
                    }
                    payload.Id = uuid.ToString();
                    payload.ApplDate = DateTime.Now;
                    payload.Stage = "1";
                    payload.Status = "Pending";
                    var data = await _repo.CreateCandidate(payload!);

                    var sample = new
                    {
                        code = 200,
                        data
                    };

                    return Ok(sample);
                }
                else if (dateDifference < 6570)
                {
                    var response = new
                    {
                        code = 401,
                        message = "Applicant is less than 18 years of age",
                    };

                    return Ok(response);
                }
                else if (mail.Any())
                {
                    var response = new
                    {
                        code = 401,
                        message = "Applicant Email already exists",
                    };

                    return Ok(response);
                }

                else
                {
                    var response = new
                    {
                        code = 401,
                        message = "Invalid Request Payload",
                    };

                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, e.Message);
            }
        }

        else
        {
            return StatusCode(500, "Invalid request body");
        }
    }

    [HttpPost("mail")]
    public ActionResult SendMail(EmailDto payload)
    {
        try
        {
            // var token  = "127b6cf109ea9f85b0f728d402de84d6bba4e4e5b2fa76d196a732fa4b74dbc4441d1cb0d6d03e058129b5300d75557dc26d975d591b3e0ea72144609d9b520e36c0c99626ece04c75b7f14135ae7d3431a6c229498b1e4cf82f10f877fadf07f42318e6365edfa89107cc6d2d39ee640b2fbcb443e0c94a26ff5cbee03777466db0306eb22226b90a50d1ccda6c92cd";
            // var key = "ec!a!7i_?!92(^wl.xc(gm4.?^l.!3.(";
            // var iv = "qq^9!h69(4.4.twn";
            // var userId = "72203447f8292549e12680877545";


            // HttpClient client = new();
            // client.DefaultRequestHeaders.Add("x-lapo-eve-proc", Credentials.token);

            // var content = JsonSerializer.Serialize(payload);

            // var encryptedBody = AEShandler.Encrypt(content, key, iv);

            // byte[] bytes = Convert.FromBase64String(encryptedBody);

            // byte[] stringBytes = Convert.FromHexString(token);

            // string res = Convert.ToBase64String(stringBytes);

            // var decrypt = AEShandler.Decrypt(res, key, iv);

            // string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();


            // var cred = await  Credentials.Renew();

            // Console.WriteLine(cred);
            // Console.WriteLine(hex);

            //     using StringContent jsonContent = new(
            // content: JsonSerializer.Serialize(hexString),
            //     Encoding.UTF8,
            //     "application/json");


            //     using HttpResponseMessage response = await client.PostAsync(
            //         "http://10.0.0.184:8023/sendmail",
            //         jsonContent);
            //     var jsonResponse = await response.Content.ReadAsStringAsync();
            //     Console.WriteLine(jsonResponse);
            //     if (response.IsSuccessStatusCode)
            //     {
            //         Console.WriteLine($"{jsonResponse}\n");
            //     }
            //     return Ok(jsonResponse);
            _repo.SendMail(payload);
            var response = new {
                code = 200,
                message = "Mail sent"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("skills/{id}")]
    public async Task<ActionResult<List<string>>> GetSkills(string id)
    {
        try
        {
            var data = await _repo.GetSkills(id);

            var response = new
            {
                code = 200,
                data
            };

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("flag")]
    public async Task<ActionResult<dynamic>> FlagCandidate(FlagCandidateDto payload)
    {
        try
        {
            var data = await _repo.FlagCandidate(payload);

            var response = new
            {
                code = 200,
                message = "Successfull"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("flag/candidates")]
    public async Task<ActionResult<List<CandidateModel>>> GetByFlag(CandidateByFlagDto payload)
    {
        try
        {
            var data = await _repo.GetByFlag(payload);

            var response = new
            {
                code = 200,
                message = "Successfull",
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

    [HttpPost("skills/candidates")]
    public async Task<ActionResult<List<string>>> GetCandidateBySkill(SkillsInput payload)
    {
        try
        {
            var data = await _repo.GetCandidateBySkills(payload);

            var response = new
            {
                code = 200,
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

    [HttpPost("/upload")]
    public async Task<ActionResult> UploadCv(IFormFile cv)
    {
        try
        {
            Console.WriteLine(cv.FileName);
            if (cv != null)
            {
                Console.WriteLine("theres something alright");
                var data = await _repo.ParseCvAsync(cv, guid);
                var cvMetaData = await _repo.ParseCvData(cv);
                var response = new
                {
                    code = 200,
                    message = "success",
                    data,
                    cvMeta = cvMetaData
                };

                return Ok(response);
            }
            return Ok(new { code = 500, message = "Unsuccessful" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("/stage")]
    public async Task<ActionResult> UpdateStage(UpdateRole payload)
    {
        try
        {
            var data = await _repo.UpdateStage(payload);

            var response = new
            {
                code = 200,
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

    [HttpGet("role/{id}")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateByRole(string id)
    {
        try
        {
            var data = await _repo.GetCandidatesByRole(id);
            var response = new
            {
                code = 200,
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

    [HttpPut("email")]
    public async Task<ActionResult<List<CandidateModel>>> UpdateData(UpdateEmail payload)
    {
        try
        {
            var data = await _repo.UpdateData(payload);

            var sample = new
            {
                code = 200,
                message = "email updated successfully"
            };

            return Ok(sample);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("cancel")]
    public async Task<ActionResult> CancelApplication(CancelApplication payload)
    {
        try
        {
            var data = await _repo.CancelApplication(payload);

            var response = new
            {
                code = 200,
                message = "Application Cancelled"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("/status")]
    public async Task<ActionResult<CandidateModel>> GetStatus(GetStatus payload)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var data = await _repo.GetStatus(payload);

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
        else
        {
            return StatusCode(500, "Invalid request body");
        }
    }


    [HttpGet("/resume/{id}")]
    public ActionResult<dynamic> GetResume(string id)
    {
        try
        {
            // var path = $"wwwroot/cv/7e4fc820-30dd-4d08-85b1-93f338a7f683.pdf";
            dynamic path2 = $"C:/Users/LAPO Mfb/Desktop/cv/{id}..pdf";
            var extension = Path.GetExtension(path2);
            var content = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(content, "application/octet-stream");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }
}