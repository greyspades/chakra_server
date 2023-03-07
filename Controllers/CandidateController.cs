using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Recruitment.Interface;
using Microsoft.AspNetCore.Cors;

namespace Candidate.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly ICandidateRepository _repo;
    public CandidateController(ICandidateRepository repo)
    {
        this._repo = repo;
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

    [HttpGet("test")]
    public async Task<ActionResult<List<CandidateModel>>> TestEndpoint()
    {
        try
        {
            var sample = new
            {
                code = 200,
                message = "ludex gundyr"
            };

            return Ok(sample);
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
    public async Task<ActionResult<List<CandidateModel>>> CreateCandidate(CandidateModel payload)
    {
        try
        {
            payload.Id = guid.ToString();
            payload.ApplicationDate = DateTime.Now;
            payload.Stage = "1";
            payload.Status = "Pending";

            var data = await _repo.CreateCandidate(payload);

            var  sample = new
            {
                code = 200,
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

    [HttpPost("/stage")]
    public async Task<ActionResult> UpdateStage(UpdateRole payload) {
        try {
            var data = await _repo.UpdateStage(payload);
            
            var  response = new
            {
                code = 200,
                data
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("role/{id}")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateByRole(string id) {
        try {
            var data = await _repo.GetCandidatesByRole(id);
            var response = new {
                code = 200,
                data
                };
            return Ok(response);
        }
        catch(Exception e) {
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
    public async Task<ActionResult> CancelApplication(string id) {
        try {
            var data = await _repo.CancelApplication(id);

        var response = new {
            code = 200,
            message = "Application Cancelled"
        };

        return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    } 
}