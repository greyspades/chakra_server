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
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;
using Quartz.Impl.Matchers;
using Credentials.Models;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Localization;
using AES;
using HTML;
using CredentialsHandler;
using System.Net.Mime;
using Meetings.Models;
using Roles.Models;
using System.Collections;
using System.Security.Policy;

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

        var data = await connection.ExecuteAsync("INSERT into Candidates(id, firstname, lastname, email, stage, roleid, status, dob, applDate, password, phone, experience, cvpath, cvextension, education, gender, otherName, coverletter, jobname) VALUES (@id, @FirstName, @LastName, @Email, 1, @RoleId, 'Pending', @Dob, @ApplDate, @Password, @Phone, @Experience, @CvPath, @CvExtension, @Education, @Gender, @OtherName, @CoverLetter, @JobName)", payload);

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
    public async Task<string> HireCandidate(HireDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var currentId = await connection.QueryAsync<string>("SELECT currentid from tempid WHERE id = '100'");

        var summation = int.Parse(currentId.FirstOrDefault()!) + 1;

        var tempId = summation.ToString().PadLeft(5, '0');

        Console.WriteLine(tempId);

        await connection.ExecuteAsync("UPDATE candidates SET tempid = @TempId WHERE id = @Id", new { Id = payload.Id, TempId = tempId });

        await connection.ExecuteAsync("UPDATE tempid SET currentid = @TempId WHERE id = '100'", new { TempId = tempId });

        await connection.ExecuteAsync("UPDATE candidates SET status = 'Hired' WHERE id = @Id", payload);

        return $"TSN{tempId}";
    }
    public async Task<IEnumerable<BasicInfo>> CheckEmail(BasicInfo payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("SELECT * from basicinfo WHERE email = @Email", payload);

        return data;
    }
    public async Task<CredentialsObj> GetCredentials()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CredentialsObj>("SELECT token, aeskey, aesiv FROM tokens WHERE id = '100'");

        return data.First();
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
    public async Task<IEnumerable<CandidateModel>> GetStatus(GetStatusDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email", payload);

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
    public async Task<IEnumerable<CandidateModel>> CheckCandidate(CandidateModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * from candidates WHERE email = @Email and roleid = @RoleId", payload);

        return data;
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
    public async Task<MeetingDto> CreateMeeting(MeetingDto payload)
    {

        HttpClient client = new();

        var token = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOm51bGwsImlzcyI6Ik9uaTBjaFdvUWZ5eExfbVdMVm1pVFEiLCJleHAiOjE2ODI5NTU2NDcsImlhdCI6MTY4MjM1MDg1MH0.vKkgpf31QmZjezswfZoEbWHcnkLq3j0qormI987GCdo";

        client.DefaultRequestHeaders.Add("authorization", "bearer" + token);

        var body = new
        {
            topic = payload.Topic,
            type = 2,
            start_time = $"{payload.Date}T{payload.Time}:00",
            //   start_time = "2023-06-21T09:20:00",
            duration = "45",
            timezone = "UTC",
            agenda = payload.Topic,
            recurrence = new
            {
                type = 1,
                repeat_interval = 1
            },
            settings = new
            {
                host_video = "true",
                participant_video = "true",
                join_before_host = "False",
                mute_upon_entry = "False",
                watermark = "true",
                audio = "voip",
                auto_recording = "cloud"
            }
        };
        using StringContent jsonContent = new(
            content: JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

        using HttpResponseMessage response = await client.PostAsync("https://api.zoom.us/v2/users/me/meetings", jsonContent);

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jObject = JObject.Parse(jsonResponse);

        var data = new MeetingDto
        {
            Completed = "false",
            Date = jObject.Value<string>("start_time")?.Split("T")[0],
            Time = payload.Time,
            MeetingId = jObject.Value<string>("id"),
            ParticipantId = payload.ParticipantId,
            Password = jObject.Value<string>("password"),
            Topic = payload.Topic,
            Link = jObject.Value<string>("join_url"),
            Email = payload.Email,
            FirstName = payload.FirstName,
            JobTitle = payload.JobTitle,
            JobId = payload.JobId,
            LastName = payload.LastName
        };

        Console.WriteLine(data.Date);

        return data;
    }
    public async Task StoreSessionInfo(MeetingDto payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("INSERT into meetings(meetingid, password, participantid, topic, date, time, completed, link, jobId, firstname, jobtitle, email, lastname) VALUES(@MeetingId, @Password, @ParticipantId, @Topic, @Date, @Time, @Completed, @Link, @JobId, @FirstName, @JobTitle, @Email, @LastName)", payload);
    }
    public async Task<string> SendMail(EmailDto payload, CredentialsObj cred)
    {

        HttpClient client = new();

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", cred.Token);

        var content = JsonSerializer.Serialize(payload);

        var encryptedBody = AEShandler.Encrypt(content, cred.AesKey, cred.AesIv);

        byte[] bytes = Convert.FromBase64String(encryptedBody);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        using MultipartFormDataContent multipartContent = new()
        {
            { new StringContent(hexString, Encoding.UTF8, MediaTypeNames.Text.Plain), "xPayload" }
        };

        using HttpResponseMessage response = await client.PostAsync(
            "http://10.0.0.184:8023/sendmail-raw", multipartContent
        );

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return jsonResponse;
    }

    public async Task<IEnumerable<dynamic>> GetJobDescription(string code)
    {
        HttpClient client = new();

        var cred = new CredHandler(_config);

        var credData = await cred.MakeContract();

        var token = new
        {
            tk = credData?[0],
            src = "AS-IN-D659B-e3M",
            rl = "118",
            us = "0C9C4760-F96C-42AF-80B0-B29B9EDEF237"
        };

        var body = new
        {
            xParam = "", 
            xScope = "Self",
            xJFCode = code
        };

        var jsonHeader = JsonSerializer.Serialize(token);

        var jsonBody = JsonSerializer.Serialize(body);

        var encryptedBody = AEShandler.Encrypt(jsonBody, credData?[1], credData?[2]);

        var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

        byte[] bodyBytes = Convert.FromBase64String(encryptedBody);

        byte[] bytes = Convert.FromBase64String(encryptedHeader);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        string bodyHexString = BitConverter.ToString(bodyBytes).Replace("-", "").ToLower();

        using StringContent content = new(
            content: bodyHexString,
                Encoding.UTF8,
                "application/json");

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

        using HttpResponseMessage response = await client.PostAsync("http://10.0.0.184:8015/performance/admin/retrievejobresponsibilitieslist", content);

        var resData = await response.Content.ReadAsStringAsync();

        Console.WriteLine(resData);

        var jsonData = JObject.Parse(resData);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

            var data = JsonSerializer.Deserialize<IEnumerable<dynamic>>(decrypted);

            return data;
        }

        return Array.Empty<IEnumerable<dynamic>>();
    }

    public async Task<IEnumerable<dynamic>> GetJobRoles()
    {

        HttpClient client = new();

        var cred = new CredHandler(_config);

        var credData = await cred.MakeContract();

        var token = new
        {
            tk = credData?[0],
            src = "AS-IN-D659B-e3M",
            rl = "118",
            us = "0C9C4760-F96C-42AF-80B0-B29B9EDEF237"
        };

        var jsonHeader = JsonSerializer.Serialize(token);

        var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

        byte[] bytes = Convert.FromBase64String(encryptedHeader);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

        var default1 = "http://10.0.0.184:8015/shared/retrievejobfunctions/all/retrievejobfunctions";
        var description = "http://10.0.0.184:8015/performance/admin/retrievejobresponsibilitieslist";

        using HttpResponseMessage response = await client.GetAsync(default1);

        var resData = await response.Content.ReadAsStringAsync();

        var jsonData = JObject.Parse(resData);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

            var data = JsonSerializer.Deserialize<IEnumerable<dynamic>>(decrypted);

            return data;
        }

        return Array.Empty<IEnumerable<dynamic>>();
    }
    public async Task<IEnumerable<MeetingDto>> GetMeetings()
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("SELECT * FROM meetings WHERE completed = 'false'");

        return data;
    }
    public async Task<IEnumerable<MeetingDto>> GetMeetingsByJob(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("SELECT * FROM meetings WHERE completed = 'false' AND jobid = @Id", new { Id = id });

        return data;
    }

    public async Task<IEnumerable<MeetingDto>> GetMeeting(MeetingDto payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("SELECT * from meetings WHERE time = @Time AND date = @Date", payload);

        return data;
    }

    public async Task<IEnumerable<CandidateModel>> GetCandidateByStage(StageDto payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        IEnumerable<CandidateModel>? data = null;

        if (payload.RoleId == "All")
        {
            data = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates WHERE stage = @Stage", payload);
        }
        else
        {
            data = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates WHERE stage = @Stage AND roleid = @RoleId", payload);
        }

        return data;
    }

    public async Task<dynamic> GetMetrics()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var finalists = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates WHERE stage = '3'");

        var applications = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates");

        var jobRoles = await connection.QueryAsync<RoleModel>("SELECT * FROM roles");

        var hired = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates WHERE status = 'Hired'");

        var result = new
        {
            finalists = finalists.Count(),
            applications = applications.Count(),
            jobRoles = jobRoles.Count(),
            hired = hired.Count()
        };

        return result;
    }

    public async Task<IEnumerable<RoleModel>> GetJobByCode(string code)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<RoleModel>("SELECT * FROM roles WHERE code = @Code", new { Code = code });

        return data;
    }

    public async Task<dynamic> CreateUser(BasicInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("INSERT INTO basicInfo(firstname, lastname, othername, phone, email, dob, password, address, gender, maritalstatus, id) VALUES (@FirstName, @LastName, @OtherName, @Phone, @Email, @Dob, @Password, @Address, @Gender, @MaritalStatus, @Id)", payload);

        return data;
    }

    public async Task<IEnumerable<BasicInfo>> GetBasicInfo(string email)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("SELECT * FROM basicinfo WHERE email = @Email", new { Email = email });

        return data;

    }
    public async Task<dynamic> AdminAuth(AdminDto payload)
    {

        HttpClient client = new();

        var cred = new CredHandler(_config);

        var credData = await cred.MakeContract();

        // Console.Write(credData?[0]);

        var token = new
        {
            tk = credData?[0],
            src = "AS-IN-D659B-e3M",
        };

        var body = new
        {
            UsN = payload.Id,
            Pwd = payload.Password,
            xAppSource = "AS-IN-D659B-e3M"
        };

        var jsonBody = JsonSerializer.Serialize(body);

        var encryptedBody = AEShandler.Encrypt(jsonBody, credData?[1], credData?[2]);

        byte[] bodyBytes = Convert.FromBase64String(encryptedBody);

        string bodyHexString = BitConverter.ToString(bodyBytes).Replace("-", "").ToLower();

        var jsonHeader = JsonSerializer.Serialize(token);

        var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

        byte[] bytes = Convert.FromBase64String(encryptedHeader);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        using StringContent jsonContent = new(
        bodyHexString,
        Encoding.UTF8,
        "application/json");

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

        using HttpResponseMessage response = await client.PostAsync("http://10.0.0.184:8015/userservices/mobile/authenticatem", jsonContent);

        var resData = await response.Content.ReadAsStringAsync();

        // Console.WriteLine(resData);

        var jsonData = JObject.Parse(resData);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

            var data = JsonSerializer.Deserialize<dynamic>(decrypted);

            var res = new
            {
                code = 200,
                message = "Successful",
                data
            };

            return res;
        }
        else if (jsonData.Value<string>("status") != "200")
        {
            Console.WriteLine("something went wrong");
            var res = new
            {
                code = 400,
                message = jsonData.Value<string>("message_description"),
            };
            return res;
        }

        return Array.Empty<IEnumerable<dynamic>>();
    }

    public async Task<IEnumerable<CandidateModel>> GetCandidateByMail(string mail)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("SELECT * FROM candidates WHERE email = @Email", new { Email = mail });

        return data;
    }

    public async Task<int> ConfirmEmail(string email) {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("UPDATE basicinfo SET emailValid = 'True' WHERE email = @Email", new { Email = email});

        return data;

    }
}
