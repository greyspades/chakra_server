using Recruitment.Interface;
using Recruitment.Context;
using Candidate.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using System.Drawing.Printing;
using Sovren;
using Sovren.Models;
using Sovren.Models.API.Parsing;
using Sovren.Models.Resume.Education;
using System.Linq;

namespace Recruitment.Repositories;
public class CandidateRepository : ICandidateRepository
{
    private readonly IConfiguration _config;
    public CandidateRepository(IConfiguration config)
    {
        this._config = config;
    }
    public async Task<IEnumerable<CandidateModel>> GetCandidates()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<CandidateModel>("Select * from Candidates");

        return data;
    }
    public async Task<IEnumerable<CandidateModel>> GetCandidateById(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("getCandidates", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<string> CreateCandidate(CandidateModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("INSERT into Candidates(id, firstname, lastname, email, stage, roleid, status, dob, applDate, password, phone) VALUES (@id, @FirstName, @LastName, @Email, 1, @RoleId, 'Pending', @Dob, @ApplicationDate, @Password, @Phone)", payload);

        return "Successful";
    }
    public async Task<string> UpdateData(UpdateEmail payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE Candidates SET email = @Email WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetCandidatesByRole(string id) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE roleid = @Id", new { Id = id});

        return data;
    }
    public async Task<string> UpdateStage(UpdateRole payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET stage = @Stage WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> CheckEmail(string mail) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email", new { Email = mail});

        return data;
    }
    public async Task<string> CancelApplication(CancelApplication payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET status = 'Canceled' WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetStatus(GetStatus payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email and password = @Password", payload);

        return data;
    }

    public async Task<string> ParseCv(IFormFile formFile) {
        var filePath = Path.Combine("C:/Users/LAPO Mfb/Desktop/cv");
        if(!Directory.Exists(filePath)) {
            Directory.CreateDirectory(filePath);
        }
        string path = filePath;

        using (Stream stream = new FileStream(filePath, FileMode.Create)){
            await formFile.CopyToAsync(stream);
        }
        Console.WriteLine(path);

        // using var stream = File.Create(filePath);
        // await formFile.CopyToAsync(stream);
        // Console.WriteLine(filePath);

    // Process uploaded files
    // Don't rely on or trust the FileName property without validation.
    // Console.WriteLine(new { count = files.Count, size });

    return path;
    }
    public async Task<dynamic> ParseCvData(IFormFile cv) {
        SovrenClient client = new SovrenClient("39626765", "Gz5jNQ3fSzfuClZuAo1RjqOaJkKBsKSN6pHc0KK/", DataCenter.EU);

        var path = "C:/Users/LAPO Mfb/Desktop/cv/resume.pdf";

        Document doc = new(path);

        // Document doc = new Document('');

        ParseRequest request = new ParseRequest(doc, new ParseOptions());

        try
    {
        ParseResumeResponse response = await client.ParseResume(request);
        Console.WriteLine("made a successful request" );
        //if we get here, it was 200-OK and all operations succeeded

        //now we can use the response from Sovren to output some of the data from the resume
        if(response != null) {
            var res = new {
            firstname = response.EasyAccess().GetCandidateName().GivenName,
            lastname = response.EasyAccess().GetCandidateName().FamilyName,
            email = response.EasyAccess().GetEmailAddresses()?.FirstOrDefault<dynamic>(),
            // education = response.EasyAccess().GetAllEducationFocusAreas()?.FirstOrDefault<dynamic>(),
            employers = response.EasyAccess().GetAllEmployers()?.FirstOrDefault<dynamic>(),

            roles = response.EasyAccess().GetAllJobTitles()?.FirstOrDefault<dynamic>(),

            experienx = response?.Value?.ResumeData?.EmploymentHistory?.Positions,

            degree = response.EasyAccess().GetHighestDegree()?.Name,
            contact = response.EasyAccess().GetContactInfo()?.Telephones?.FirstOrDefault<dynamic>(),
            address = response.EasyAccess().GetContactInfo()?.Location?.Regions?.FirstOrDefault<dynamic>(),
        };

        return res;
        }
        else {
            return new {
                code = 500,
                message = "could not process the request"
            };
        }

        
    }
    catch (SovrenException e)
    {
        //the document could not be parsed, always try/catch for SovrenExceptions when using SovrenClient
        Console.WriteLine($"Error: {e.SovrenErrorCode}, Message: {e.Message}");

        return e.Message;
    }
    }

    // public async Task<ActionResult<File>> GetResume(string id) {

    // //     var path = "<Get the file path using the ID>";
    // // var stream = File.OpenRead(path);
    // // return new FileStreamResult(stream, "application/octet-stream");

    //     var content = new FileStream("C:/Users/LAPO Mfb/Desktop/cv/resume.pdf",FileMode.Open, FileAccess.Read, FileShare.Read);
    //     var response = File(content, "application/octet-stream");//FileStreamResult
    //     return response;
    // }
}
