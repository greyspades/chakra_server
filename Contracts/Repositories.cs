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
using AsposeDoc = global::Aspose.Words;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Org.BouncyCastle.Crypto;
using Quartz.Impl.Matchers;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;

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
    // public async Task<IEnumerable<string>> GetCandidateBySkills(string skill, role) {
    //     using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    //     var data = await connection.QueryAsync<string>("", new { Skill = skill });

    //     return data;
    // }
    public async Task<string> CreateCandidate(CandidateModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("INSERT into Candidates(id, firstname, lastname, email, stage, roleid, status, dob, applDate, password, phone, experience, cvpath, cvextension, education, gender, otherName) VALUES (@id, @FirstName, @LastName, @Email, 1, @RoleId, 'Pending', @Dob, @ApplDate, @Password, @Phone, @Experience, @CvPath, @CvExtension, @Education, @Gender, @OtherName)", payload);

        for (int i = 0; i < payload?.Skills.Count; i++)
        {
            await connection.ExecuteAsync("INSERT into Skills(id, skill, unit) VALUES(@Xid, @Item, @Unit)", new { Item = payload.Skills[i], Xid = payload.Id, Unit = payload.RoleId });
        };

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
    public async Task<IEnumerable<string>> GetSkills(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<string>("SELECT skill from Skills where id = @Id", new { Id = id });

        return data;
    }
    public async Task<IEnumerable<string>> GetCandidateBySkills(SkillsInput payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<string>("SELECT skill from Skills WHERE id = @Id AND unit = @Unit", payload);

        return data;
    }
    public async Task<string> UpdateStage(UpdateRole payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET stage = @Stage WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> CheckEmail(CandidateModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email AND roleid = @RoleId", payload);

        return data;
    }
    public async Task<string> CancelApplication(CancelApplication payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET status = 'Canceled' WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<string> FlagCandidate(FlagCandidateDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE candidates SET flag = @Flag WHERE id = @Id", payload);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetStatus(GetStatus payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email and password = @Password", payload);

        return data;
    }
    public async Task<IEnumerable<CandidateModel>> GetByFlag(CandidateByFlagDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * FROM Candidates WHERE roleid = @RoleId and flag = @Flag", payload);

        return data;
    }

    public async Task<dynamic> ParseCvAsync(IFormFile formFile, Guid id)
    {
        string guid = id.ToString();

        var extension = Path.GetExtension(formFile.FileName);
        var path = $"C:/Users/LAPO Mfb/Desktop/cv/{guid}.{extension}";
        var path2 = $"wwwroot/cv/{guid}.docx";
        var fileData = new
        {
            extension,
            id = guid
        };
        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await formFile.CopyToAsync(stream);
        }
        if (extension != ".pdf")
        {
            var doc = new AsposeDoc.Document(path);
            doc.Save($"C:/Users/LAPO Mfb/Desktop/cv/{guid}..pdf");
            File.Delete(path);
        }

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

    public void SendMail(EmailDto payload)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("greyspades99@gmail.com"));
        email.To.Add(MailboxAddress.Parse(payload.Reciever));
        if(payload.Template == "notFit") {
            email.Subject = "Sorry not a fit";
            email.Body = new TextPart(TextFormat.Plain) { Text = "Sorry you arent a fit for this role, you can try again later" };
        }
        else if(payload.Template == "accepted") {
            email.Subject = "You are a fit";
            email.Body = new TextPart(TextFormat.Plain) { Text = "You are a fit for this role, you may likely get it so keep your fingers crossed" };
        }
        // send email
        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("greyspades99@gmail.com", "ojatxgekghcfqdqi");
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
