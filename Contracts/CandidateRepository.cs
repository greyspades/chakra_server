using Candidate.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Credentials.Models;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using AES;
using CredentialsHandler;
using System.Net.Mime;
using Roles.Models;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Candidate.Interface;
using Spire.Doc;
using RestSharp;
using Pdf.Helper;
using SautinSoft.Document;
// using Microsoft.Office.Interop.Word;
// using Microsoft.Office.Interop.Word;
// using Word = Microsoft.Office.Interop.Word;
// using AsposePdf = global::Aspose.Pdf;
// using Aspose.Pdf.Text;
// using System.Reflection.Metadata;
// using Aspose.Pdf;

namespace Candidate.Repository;
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

        var data = await connection.QueryAsync<CandidateModel>("Get_all_applications", commandType: CommandType.StoredProcedure);

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

        var body = new
        {
            email = payload.Email,
            id = payload.Id,
            stage = payload.Stage,
            firstname = payload.FirstName,
            lastname = payload.LastName,
            experience = payload.Experience,
            education = payload.Education,
            phone = payload.Phone,
            roleid = payload.RoleId,
            status = payload.Status,
            applDate = payload.ApplDate,
            dob = payload.Dob,
            password = payload.Password,
            jobname = payload.JobName,
            coverletter = payload.CoverLetter,
            gender = payload.Gender,
            othername = payload.OtherName,
            address = payload.Address,
            maritalStatus = payload.MaritalStatus,
        };

        await connection.ExecuteAsync("Create_application", body, commandType: CommandType.StoredProcedure);

        if (payload.Skills != null)
        {
            for (int i = 0; i < payload?.Skills.Count; i++)
            {
                await connection.ExecuteAsync("Add_skills", new { Item = payload.Skills[i], Xid = payload.Id}, commandType: CommandType.StoredProcedure);
            };
        }

        return "Successful";
    }
    public async Task<string> UpdateData(UpdateEmail payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Get_jobs", payload, commandType: CommandType.StoredProcedure);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetCandidatesByRole(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        IEnumerable<CandidateModel>? data = await connection.QueryAsync<CandidateModel>("Get_jobs", new { Id }, commandType: CommandType.StoredProcedure);
        return data;
    }
    public async Task<IEnumerable<string>> GetSkills(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<string>("Get_skills", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<string>> GetCandidateBySkills(SkillsInput payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<string>("Get_candidate_by_skills", payload, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<string> UpdateStage(UpdateRole payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Update_stage", payload, commandType: CommandType.StoredProcedure);

        return "Successful";
    }
    public async Task<string> HireCandidate(HireDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var currentId = await connection.QueryAsync<string>("Get_last_temp_id", commandType: CommandType.StoredProcedure);

        var summation = int.Parse(currentId.FirstOrDefault()!) + 1;

        var tempId = summation.ToString().PadLeft(5, '0');

        await connection.ExecuteAsync("Set_temp_id", new { Id = payload.Id, TempId = tempId }, commandType: CommandType.StoredProcedure);

        await connection.ExecuteAsync("Update_last_temp_id", new { TempId = tempId }, commandType: CommandType.StoredProcedure);

        await connection.ExecuteAsync("Hire_candidate", new { Id = payload.Id }, commandType: CommandType.StoredProcedure);

        return $"TSN{tempId}";
    }
    public async Task<IEnumerable<BasicInfo>> CheckEmail(string email)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("Get_info_by_email", new { Email = email }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<CredentialsObj> GetCredentials()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CredentialsObj>("Get_cred", commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task<string> CancelApplication(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Cancel_application", new { Id = id }, commandType: CommandType.StoredProcedure);

        return "Successful";
    }
    public async Task<string> FlagCandidate(FlagCandidateDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Flag_application", new { Id = payload.Id, Flag = payload.Flag }, commandType: CommandType.StoredProcedure);

        return "Successful";
    }
    public async Task<IEnumerable<CandidateModel>> GetStatus(GetStatusDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("Get_status", payload, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<CandidateModel>> GetByFlag(CandidateByFlagDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        IEnumerable<CandidateModel> data;

        if(payload.RoleId != "") {
            Console.WriteLine("its by role");
            data = await connection.QueryAsync<CandidateModel>("Get_application_by_flag", payload, commandType: CommandType.StoredProcedure);
        } else {
            Console.WriteLine("all applications");
            data = await connection.QueryAsync<CandidateModel>("Get_all_applications_by_flag", new { payload.Flag }, commandType: CommandType.StoredProcedure);
        }

        return data;
    }

    public async Task<dynamic> ParseCvAsync(IFormFile formFile, Guid id)
    {
        string guid = id.ToString();

        var extension = Path.GetExtension(formFile.FileName);
        var path = $"cv/{guid}.{extension}";
        var fileData = new
        {
            extension,
            id = guid
        };
        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await formFile.CopyToAsync(stream);
        }
        // if (extension != ".pdf")
        // {
        //     var doc = new AsposeDoc.Document(path);
        //     doc.Save($"C:/Users/LAPO Mfb/Desktop/cv/{guid}..pdf");
        //     File.Delete(path);
        // }

        return fileData;
    }
    public async Task<IEnumerable<CandidateModel>> CheckCandidate(CandidateModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("Check_application", payload, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task<byte[]> GetBytes(IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    // public async Task<dynamic> ParseCvData(IFormFile cv)
    // {
    //     SovrenClient client = new SovrenClient("39626765", "Gz5jNQ3fSzfuClZuAo1RjqOaJkKBsKSN6pHc0KK/", DataCenter.EU);

    //     var path = "C:/Users/LAPO Mfb/Desktop/cv/resume.pdf";
    //     try
    //     {
    //         var bytes = await GetBytes(cv);

    //         Document doc = new(bytes, File.GetLastWriteTime(path));

    //         ParseRequest request = new(doc);

    //         ParseResumeResponse response = await client.ParseResume(request);
    //         Console.WriteLine("made a successful request");
    //         //if we get here, it was 200-OK and all operations succeeded

    //         //now we can use the response from Sovren to output some of the data from the resume
    //         if (response != null)
    //         {
    //             var res = new
    //             {
    //                 firstname = response.EasyAccess().GetCandidateName().GivenName,
    //                 lastname = response.EasyAccess().GetCandidateName().FamilyName,
    //                 email = response.EasyAccess().GetEmailAddresses()?.FirstOrDefault<dynamic>(),
    //                 employers = response.EasyAccess().GetAllEmployers()?.FirstOrDefault<dynamic>(),
    //                 roles = response.EasyAccess().GetAllJobTitles()?.FirstOrDefault<dynamic>(),
    //                 experience = response?.Value?.ResumeData?.EmploymentHistory?.Positions,
    //                 degree = response.EasyAccess().GetHighestDegree()?.Name,
    //                 contact = response.EasyAccess().GetContactInfo()?.Telephones?.FirstOrDefault<dynamic>(),
    //                 address = response.EasyAccess().GetContactInfo()?.Location?.Regions?.FirstOrDefault<dynamic>(),
    //                 skills = response?.Value?.ResumeData.Skills,
    //                 education = response?.Value?.ResumeData?.Education
    //             };

    //             return res;
    //         }
    //         else
    //         {
    //             return new
    //             {
    //                 code = 500,
    //                 message = "could not process the request"
    //             };
    //         }

    //     }
    //     catch (SovrenException e)
    //     {
    //         //the document could not be parsed, always try/catch for SovrenExceptions when using SovrenClient
    //         Console.WriteLine($"Error: {e.SovrenErrorCode}, Message: {e.Message}");

    //         return e.Message;
    //     }
    // }
    public async Task<string> CreateMeetingToken()
    {
        HttpClient client = new();

        var accountId = _config.GetValue<string>("Zoom:Account_id");
        var clientId = _config.GetValue<string>("zoom:Client_id");
        var clientSecret = _config.GetValue<string>("zoom:Client_secret");

        var payload = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");

        string encodedPayload = Convert.ToBase64String(payload);

        client.DefaultRequestHeaders.Add("authorization", "Basic" + $" {encodedPayload}");

        using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("Zoom:Token_url"), null);

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jObject = JObject.Parse(jsonResponse);

        var token = jObject.Value<string>("access_token");

        return token;

    }
    public async Task<MeetingDto> CreateMeeting(MeetingDto payload, string token)
    {
        HttpClient client = new();

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

        using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("Zoom:Url"), jsonContent);

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

        return data;
    }
    public async Task StoreSessionInfo(MeetingDto payload)
    {
        var body = new
        {
            meetingId = payload.MeetingId,
            password = payload.Password,
            participantId = payload.ParticipantId,
            date = payload.Date,
            time = payload.Time,
            completed = payload.Completed,
            link = payload.Link,
            jobId = payload.JobId,
            jobTitle = payload.JobTitle,
            topic = payload.Topic,
            firstname = payload.FirstName,
            lastname = payload.LastName,
            email = payload.Email
        };

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_meeting", body, commandType: CommandType.StoredProcedure);
    }
    public async Task<string> SendMail(EmailDto payload, CredentialsObj cred)
    {
        var content = JsonSerializer.Serialize(payload);

        var encryptedBody = AEShandler.Encrypt(content, cred.AesKey, cred.AesIv);

        byte[] bytes = Convert.FromBase64String(encryptedBody);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        if(payload.HasFile == "Yes") {
            var fileStream = File.OpenRead("templates/letter.pdf");
            var options = new RestClientOptions(_config.GetValue<string>("E360:Mail_url"))
            {
            MaxTimeout = -1,
            };
            var rest = new RestClient(options);
            var request = new RestRequest(_config.GetValue<string>("E360:Mail_url"), Method.Post);
            request.AddHeader("x-lapo-eve-proc", cred.Token);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("xPayload", hexString);

            request.AddFile("xFile", "templates/letter.pdf");

            RestResponse response = await rest.ExecuteAsync(request);

            return response.Content;

        } else {
            HttpClient client = new();

            client.DefaultRequestHeaders.Add("x-lapo-eve-proc", cred.Token);

            using MultipartFormDataContent multipartContent = new()
        {
            { new StringContent(hexString, Encoding.UTF8, MediaTypeNames.Text.Plain), "xPayload" }
            
        };

        using HttpResponseMessage response = await client.PostAsync(
            _config.GetValue<string>("E360:Mail_url"), multipartContent
        );

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return jsonResponse;
        }
    }
    public async Task<IEnumerable<MeetingDto>> GetMeetings(string id)
    {
        IEnumerable<MeetingDto>? data;
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        if(id == "" || id == null) {
            data = await connection.QueryAsync<MeetingDto>("Get_meetings", commandType: CommandType.StoredProcedure);
        } else {
            data = await connection.QueryAsync<MeetingDto>("Get_meetings_by_application", new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        return data;
    }
    public async Task ResetPassword(PasswordResetDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Reset_password", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<MeetingDto>> GetMeeting(MeetingDto payload)
    {
        var body = new
        {
            time = payload.Time,
            date = payload.Date
        };

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("Get_meetings_by_time", body, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task<IEnumerable<CandidateModel>> GetCandidateByStage(StageDto payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        IEnumerable<CandidateModel>? data = null;

        if (payload.RoleId == "All")
        {
            var body = new
            {
                stage = payload.Stage
            };
            data = await connection.QueryAsync<CandidateModel>("Get_applications_by_stage", body, commandType: CommandType.StoredProcedure);
        }
        else
        {
            var body = new
            {
                stage = payload.Stage,
                roleId = payload.RoleId
            };
            data = await connection.QueryAsync<CandidateModel>("Get_applications_by_stage_and_role", body, commandType: CommandType.StoredProcedure);
        }

        return data;
    }

    public async Task<dynamic> GetMetrics()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var finalists = await connection.QueryAsync<CandidateModel>("Get_finalists", commandType: CommandType.StoredProcedure);

        var applications = await connection.QueryAsync<CandidateModel>("Get_all_applications", commandType: CommandType.StoredProcedure);

        var jobRoles = await connection.QueryAsync<JobRoleModel>("Get_all_job_roles", commandType: CommandType.StoredProcedure);

        var hired = await connection.QueryAsync<CandidateModel>("Get_all_hired_applicants", commandType: CommandType.StoredProcedure);

        var result = new
        {
            finalists = finalists.Count(),
            applications = applications.Count(),
            jobRoles = jobRoles.Count(),
            hired = hired.Count()
        };

        return result;
    }

    public async Task<dynamic> CreateUser(BasicInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Create_new_user", payload, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task<IEnumerable<BasicInfo>> GetBasicInfo(string email)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("Gets_user_by_email", new { Email = email }, commandType: CommandType.StoredProcedure);

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

        using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("E360:Signin_url"), jsonContent);

        var resData = await response.Content.ReadAsStringAsync();

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
        Console.Write(mail);
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CandidateModel>("Get_application_by_email", new { Email = mail }, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task<int> ConfirmEmail(string email)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.ExecuteAsync("Validate_email", new { Email = email }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task CreateComment(CommentDto payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Creates_comment", payload, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CommentDto>> GetComments(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CommentDto>("Gets_comment_by_Application", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task<dynamic> CreateTeamsMeeting()
    {
        // The client credentials flow requires that you request the
        // /.default scope, and preconfigure your permissions on the
        // app registration in Azure. An administrator must grant consent
        // to those permissions beforehand.
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = _config.GetValue<string>("Graph_api:tenant_id");

        // Values from app registration
        var clientId = _config.GetValue<string>("Graph_api:client_id");
        var clientSecret = _config.GetValue<string>("Graph_api:client_secret");

        // using Azure.Identity;
        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

        var requestBody = new OnlineMeeting
        {
            StartDateTime = DateTimeOffset.Parse("2023-05-26T15:05:34.2444915-07:00"),
            EndDateTime = DateTimeOffset.Parse("2023-05-26T15:20:34.2464912-07:00"),
            Subject = "User Token Meeting",
        };

        var result = await graphClient.Me.OnlineMeetings.PostAsync(requestBody);

        Console.WriteLine(result);

        return result;
    }

    public async Task<IEnumerable<MeetingDto>> CheckMeetingStatus(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("check_meetings", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }

    public dynamic CreateOfferMail(HireDto payload)
    {
        var inputPath = @"templates/offer2.docx";
        var outputPath = @"templates/letter.pdf";

        object[] items = {
                new DocFields { Key = "{{position}}", Value = payload.Position},
                new DocFields { Key = "{{firstname lastname}}", Value = $"{payload.FirstName} {payload.LastName}"},
                new DocFields { Key = "{{address}}", Value = payload.Address},
                new DocFields { Key = "{{reports_to}}", Value = payload.ReportTo},
                new DocFields { Key = "{{state}}", Value = payload.City},
                new DocFields { Key = "{{salary}}", Value = payload.Salary},
                new DocFields { Key = "{location}}", Value = payload.Location},
                new DocFields { Key = "{{firstname}}", Value = payload.FirstName},
                new DocFields { Key = "{{lastname}}", Value = payload.LastName},
                new DocFields { Key = "{{date}}", Value = payload.Date.ToString().Split(" ")[0]},
                new DocFields { Key = "{{rank}}", Value = payload.Rank},
                new DocFields { Key = "{{start_date}}", Value = payload.StartDate.ToString().Split(" ")[0]},
                new DocFields { Key = "{{salwords}}", Value = payload.SalWords},
                new DocFields { Key = "{{job_type}}", Value = payload.}
            };

            Document doc = new();
            doc.LoadFromFile(@"templates/offer.docx");

            foreach(DocFields item in items) {
                doc.Replace(item.Key, item.Value, true, true);
            }
            doc.SaveToFile(@"templates/offer2.docx", FileFormat.Docx);
            
            DocumentCore dc = DocumentCore.Load(inputPath);
            dc.Save(outputPath);
            // PdfConvert.ConvertDocxToPdf("templates/offer2.docx", outputPath);

        return "Successful";
    }
}
