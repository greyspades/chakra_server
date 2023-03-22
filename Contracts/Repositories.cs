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
using MongoDB.Driver;

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

        var data = await connection.ExecuteAsync("INSERT into Candidates(id, firstname, lastname, email, stage, roleid, status, dob, appldate, password, phone, experience, skills, cvpath, cvextension, education) VALUES (@id, @FirstName, @LastName, @Email, 1, @RoleId, 'Pending', @Dob, @ApplicationDate, @Password, @Phone, @Expereince, @Skills, @CvPath, @CvExtension, @Education)", payload);

        return "Successful";
    }
    public async Task<string> UpdateData(UpdateEmail payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE Candidates SET email = @Email WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetCandidatesByRole(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE roleid = @Id", new { Id = id });

        return data;
    }
    public async Task<string> UpdateStage(UpdateRole payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET stage = @Stage WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> CheckEmail(string mail)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email", new { Email = mail });

        return data;
    }
    public async Task<string> CancelApplication(CancelApplication payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET status = 'Canceled' WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetStatus(GetStatus payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email and password = @Password", payload);

        return data;
    }

    public async Task<dynamic> ParseCv(IFormFile formFile)
    {
        var filePath = Path.Combine("wwwroot/cv", formFile.FileName);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        Guid guid = Guid.NewGuid();

        var extension = Path.GetExtension(formFile.FileName);
        Console.WriteLine(extension);

        var fileData = new
        {
            extension,
            id = guid.ToString()
        };

        // var stream = new FileStream($"wwwroot/cv/{guid}.docx", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        // await formFile.CopyToAsync(stream);

        return fileData;
    }

    public async Task<byte[]> GetBytes(IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<dynamic> ParseCvData(IFormFile cv)
    {
        SovrenClient client = new SovrenClient("39626765", "Gz5jNQ3fSzfuClZuAo1RjqOaJkKBsKSN6pHc0KK/", DataCenter.EU);

        var path = "C:/Users/LAPO Mfb/Desktop/cv/resume.pdf";
        try
        {
            var bytes = await GetBytes(cv);

            Document doc = new(bytes, File.GetLastWriteTime(path));

            ParseRequest request = new(doc);

            ParseResumeResponse response = await client.ParseResume(request);
            Console.WriteLine("made a successful request");
            //if we get here, it was 200-OK and all operations succeeded

            //now we can use the response from Sovren to output some of the data from the resume
            if (response != null)
            {
                var res = new
                {
                    firstname = response.EasyAccess().GetCandidateName().GivenName,
                    lastname = response.EasyAccess().GetCandidateName().FamilyName,
                    email = response.EasyAccess().GetEmailAddresses()?.FirstOrDefault<dynamic>(),
                    employers = response.EasyAccess().GetAllEmployers()?.FirstOrDefault<dynamic>(),
                    roles = response.EasyAccess().GetAllJobTitles()?.FirstOrDefault<dynamic>(),
                    experience = response?.Value?.ResumeData?.EmploymentHistory?.Positions,
                    degree = response.EasyAccess().GetHighestDegree()?.Name,
                    contact = response.EasyAccess().GetContactInfo()?.Telephones?.FirstOrDefault<dynamic>(),
                    address = response.EasyAccess().GetContactInfo()?.Location?.Regions?.FirstOrDefault<dynamic>(),
                    skills = response?.Value?.ResumeData.Skills,
                    education = response?.Value?.ResumeData?.Education
                };

                return res;
            }
            else
            {
                return new
                {
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
}
