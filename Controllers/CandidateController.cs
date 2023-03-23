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
using MongoDB.Helpers;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Aspose.Words;

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
                var uuid = guid;
                if(payload.Cv != null) {
                    var fileData = await _repo.ParseCvAsync(payload.Cv, uuid);
                }
                payload.Id = uuid.ToString();
                payload.ApplDate = DateTime.Now;
                payload.Stage = "1";
                payload.Status = "Pending";
                // payload.CvPath = fileData.id;
                // payload.CvExtension = fileData.extension;

                var dateDifference = (DateTime.Now.Date - payload?.Dob.Value)?.TotalDays;

                var mail = await _repo.CheckEmail(payload.Email);
                
                if (
                    // dateDifference > 6570 && 
                !mail.Any())
                {
                    Console.WriteLine("got there");
                    var data = await _repo.CreateCandidate(payload);

                    var sample = new
                    {
                        code = 200,
                        data
                    };

                    return Ok(sample);
                }
                // else if (dateDifference < 6570)
                // {
                //     var response = new
                //     {
                //         code = 401,
                //         message = "Applicant is less than 18 years of age",
                //     };

                //     return Ok(response);
                // }
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

    [HttpGet("skills/{id}")]
    public async Task<ActionResult<List<string>>> GetSkills(string id) {
        try {
            var data = await _repo.GetSkills(id);

            var response = new {
                code = 200,
                data
            };

            return Ok(data);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        } 
    }

    [HttpPost("/upload")]
    public async Task<ActionResult> UploadCv(IFormFile cv) {
        try {
            Console.WriteLine(cv.FileName);
                if(cv != null) {
                    Console.WriteLine("theres something alright");
                    var data = await _repo.ParseCvAsync(cv, guid);
                    var cvMetaData = await _repo.ParseCvData(cv);
                     var response = new  {
                        code = 200,
                        message = "success",
                        data,
                        cvMeta = cvMetaData
                     };

                     return Ok(response);
                }
            return Ok(new { code = 500, message = "Unsuccessful"});
        }
        catch (Exception e) {
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

    [HttpPost("/cancel")]
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
    public async Task<ActionResult<CandidateModel>> GetStatus(GetStatus payload) {
        if(ModelState.IsValid) {
            try {
            var data = await _repo.GetStatus(payload);

            var response = new {
                code = 200,
                message = "Successful",
                data
            };

            return Ok(response);
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
        }
        else {
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