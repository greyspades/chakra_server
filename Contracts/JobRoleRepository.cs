using Candidate.Interface;
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
using JobRole.Interface;

namespace Jobrole.Repository;

public class JobRoleRepository: IJobRoleRepository
{
    private readonly IConfiguration _config;
    public JobRoleRepository(IConfiguration config)
    {
        this._config = config;
    }

    public async Task<IEnumerable<JobRoleModel>> GetRoles()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<JobRoleModel>("SELECT * FROM Roles");

        return data;
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

        using HttpResponseMessage response = await client.PostAsync("http://10.0.0.184:8015/performance/retrievejobresponsibilitieslist", content);

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

            var data = JsonSerializer.Deserialize<IEnumerable<Job>>(decrypted);

            return data;
        }

        return Array.Empty<IEnumerable<Job>>();
    }

        public async Task<IEnumerable<MeetingDto>> GetMeetingsByJob(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<MeetingDto>("SELECT * FROM meetings WHERE completed = 'false' AND jobid = @Id", new { Id = id });

        return data;
    }

        public async Task<IEnumerable<JobRoleModel>> GetJobByCode(string code)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<JobRoleModel>("SELECT * FROM roles WHERE code = @Code", new { Code = code });

        return data;
    }

    public async Task<IEnumerable<JobRoleModel>> GetRoleByDivision(string unit)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<JobRoleModel>("SELECT * FROM roles where unit = @Unit", new { Unit = unit });

        return data;
    }

    public async Task AddJobRole(JobRoleModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("INSERT into Roles(id, name, description, experience, deadline, unit, code, status) VALUES (@Id, @Name, @Description, @Experience, @Deadline, @Unit, @Code, @Status)", payload);
    }
    public async Task<JobRoleModel> GetJobRoleById(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync("SELECT * FROM roles where id = @Id", new { Id = id });

        return data.First();
    }

    public async Task<JobRoleModel> GetJobRoleByUnit(string unit)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<JobRoleModel>("SELECT * FROM roles where unit = @Unit", new { Unit = unit });

        return data.First();
    }
}