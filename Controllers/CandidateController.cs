using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Microsoft.AspNetCore.Cors;
using System.Security;
using Microsoft.AspNetCore.StaticFiles;
using Resume.Models;
// using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AES;
using Credentials.Models;
using HTML;
using Hashing;
using BC = BCrypt.Net.BCrypt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Candidate.Interface;

namespace Candidate.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly ICandidateRepository _repo;
    public IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    // private readonly IMongoCollection<ResumeModel> _collection;
    // private readonly IMongoDatabase _mongoDb;
    private static readonly HttpClient client = new();
    public CandidateController(IWebHostEnvironment environment, ICandidateRepository repo, IConfiguration config)
    {
        this._repo = repo;
        this._env = environment;
        this._config = config;
        // var mongoClient = new MongoClient(
        //     dbSettings.Value.ConnectionString);

        // var mongoDatabase = mongoClient.GetDatabase(
        //     dbSettings.Value.DatabaseName);

        // _collection = mongoDatabase.GetCollection<ResumeModel>(
        //     dbSettings.Value.CollectionName);
        // _mongoDb = mongoDatabase;
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

    [HttpPost("id")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateById(CandidateDto payload)
    {
        try
        {
            var data = await _repo.GetCandidateById(payload.Id);

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

    [HttpPost("create")]
    public async Task<ActionResult<List<CandidateModel>>> CreateCandidate([FromForm] CandidateModel payload)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var dateDifference = (DateTime.Now.Date - payload?.Dob.Value)?.TotalDays;

                var info = await _repo.GetBasicInfo(payload?.Email);

                var applications = await _repo.GetCandidateByMail(payload.Email);

                for(int i = 0; i<applications.Count(); i++) {
                    if(applications.ElementAt(i).RoleId == payload.RoleId) {
                        var response = new {
                            code = 400,
                            message = "You have already applied for this job role"
                        };

                        return Ok(response);
                    }
                }
                // if(info.First().EmailValid != "True") {
                //     var response = new {
                //         code = 400,
                //         message = "Sorry please verify your email to continue"
                //     };

                //     return Ok(response);
                // }
                if(
                    dateDifference > 6570
                )
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

                    var data = await _repo.CreateCandidate(payload);

                    CredentialsObj cred = await _repo.GetCredentials();

                    var mailObj = new EmailDto {
                        Firstname = payload.FirstName,
                        EmailAddress = payload.Email,
                        Subject = "Thank you for applying to LAPO Microfinance Bank",
                        HasFile = "No",
                        Body = HTMLHelper.Acknowledgement(payload),
                    };
    
                    var mail = await _repo.SendMail(mailObj, cred);

                    var response = new
                    {
                        code = 200,
                        message = "Application created successfully"
                    };

                    return Ok(response);
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
    [HttpGet("token")]
    public async Task<ActionResult> CreateMeetingToken() {
        try {
            var data = await _repo.CreateMeetingToken();

            return Ok(data);
        }
        catch(Exception e) {

            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost("mail")]
    public async Task<ActionResult<string>> SendMail(EmailDto payload)
    {
        try
        {
            CredentialsObj cred = await _repo.GetCredentials();

            var data = await _repo.SendMail(payload, cred);

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("skills")]
    public async Task<ActionResult<List<string>>> GetSkills(CandidateDto payload)
    {
        try
        {

            var data = await _repo.GetSkills(payload.Id);

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

            if(payload.Flag == "notFit") {
                CredentialsObj cred = await _repo.GetCredentials();

                var candidate = await _repo.GetCandidateById(payload.Id);

                var candidateData = candidate.First();

                await _repo.CancelApplication(payload.Id);

                var mailObj = new EmailDto {
                Firstname = candidate.First().FirstName,
                EmailAddress = candidate.First().Email,
                Subject = $"Your application for {payload.RoleName} at LAPO Microfinance Bank",
                HasFile = "No",
                Body = HTMLHelper.Rejection(payload, candidateData)
                };

                var mail = await _repo.SendMail(mailObj, cred);
            }

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

    [HttpPost("hire")]
    public async Task<ActionResult> HireCandidate (HireDto payload) {
        try {
            var can = await _repo.GetCandidateById(payload?.Id);

            var canData = can.First();

            var basic = await _repo.GetBasicInfo(canData.Email);

            var basicInfo = basic.First();
            
            if(canData.TempId == null) {

                // var data = await _repo.HireCandidate(payload);

                payload.Address = basicInfo.Address;
                payload.FirstName = canData.FirstName;
                payload.LastName = canData.LastName;
                payload.Position = canData.JobName;

                var mail = _repo.SendOfferMail(payload);

                var response = new {
                code = 200,
                message = "Candidate has been hired successfully",
                mail
                };

                return Ok(response);
            }
            else {
                var response = new {
                code = 401,
                message = "Candidate has already been hired",
                };

                return Ok(response);
            }
        }
        catch (Exception e) {
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
                var data = await _repo.ParseCvAsync(cv, guid);
                // var cvMetaData = await _repo.ParseCvData(cv);
                var response = new
                {
                    code = 200,
                    message = "success",
                    data,
                    // cvMeta = cvMetaData
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
    // [HttpGet("ref")]
    // public async Task<ActionResult> GetRef() {
    //     try {

    //     } catch(Exception e) {
    //         Console.WriteLine(e.Message);

    //         var response = new {
    //             code = 200,
    //             message = "An error occured processing the request"
    //         };

    //         return StatusCode(500, response);
    //     }
    // }

    [HttpPost("/stage")]
    public async Task<ActionResult> UpdateStage(UpdateRole payload)
    {
        try
        {
            var candidate = await _repo.GetCandidateById(payload.Id);
            if(candidate.First().Stage != payload.Stage) {
                var data = await _repo.UpdateStage(payload);
                
                 var response = new
                {
                    code = 200,
                    message = "Successfully moved to the next stage"
                };

            return Ok(response);
            }
            else {
            var response = new
            {
                code = 400,
                message = "Applicant is already in stage 3"
            };

            return Ok(response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            var response = new
            {
                code = 500,
                message = "An Error Occured"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("role")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateByRole(GetCandidatesDto payload)
    {
        try
        {
            var data = await _repo.GetCandidatesByRole(payload.Id);

            var count = payload.Page * 10;
            
            var slicedCandidates = data.Skip((int)count!).Take((int)payload.Take!);

            var response = new
            {
                code = 200,
                data = slicedCandidates,
                count = data.Count()
            };

            return Ok(response);
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
            var data = await _repo.CancelApplication(payload.Id);

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

    // [Authorize]
    [HttpPost("/status")]
    public async Task<ActionResult<CandidateModel>> GetStatus(GetStatusDto payload)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var info = await _repo.GetBasicInfo(payload?.Email);

                Console.Write(info.First().LastName);

                if(info.Any()) {
                    var data = await _repo.GetStatus(payload);


                    var response = new
                {
                    code = 200,
                    message = "Successful",
                    data
                };
                return Ok(response);
                }
                else if(info.First() == null) {
                     var response = new
                {
                    code = 400,
                    message = "This Candidate does not exist, please check your email address and try again",

                };
                return Ok(response);
                }
                else {
                    var response = new
                {
                    code = 401,
                    message = "An error occured",

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
    [HttpPost("mail_reset_options")]
    public async Task<ActionResult> MailResetOptions(CandidateEmailDto payload) {
        try {
            var candidate = await _repo.GetCandidateByMail(payload.Email);
            
                if(candidate.Any()) {
                    CredentialsObj cred = await _repo.GetCredentials();

                    var encryptedEmail = AEShandler.Encrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                    var resetObj = new PasswordResetFields {
                        Link = AEShandler.Encrypt($"localhost:3000/password_reset?email={encryptedEmail}&token=00727143910", _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv")),
                        FirstName = candidate.First().FirstName
                    };

                    var mailObj = new EmailDto {
                        Firstname = candidate.First().FirstName,
                        EmailAddress = payload.Email,
                        Subject = "Reset your password",
                        HasFile = "No",
                        Body = HTMLHelper.PasswordReset(resetObj),
                    };
    
                    var mailRes = await _repo.SendMail(mailObj, cred);

                    var response = new {
                        code = 200,
                        message = "Please click the link sent to your email to reset your password",
                        mail = mailRes
                    };

                    return Ok(response);
                }
                else {
                    var response = new {
                        code = 404,
                        message = "That Email address does not exist on the system",

                    };

                    return Ok(response);
                }
            
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Sorry an error occured processing your request"
            };
            
            return StatusCode(500, response);
        }
    }

    [HttpPost("reset_password")]
    public async Task<ActionResult> ResetPassword(PasswordResetDto payload) {
        try {
            var mail = await _repo.CheckEmail(payload.Email);
            if(mail.Any()) {
                
                await _repo.ResetPassword(payload);

                var response = new {
                    code = 200,
                    message = "Your password has been updated successfully"
                };

            return Ok(response);
            } else {
                var response = new {
                    code = 404,
                    message = "Sorry that email address does not exist on our records"
                };

            return Ok(response);
            }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Sorry could not process your request"
            };

            return StatusCode(500, response);
        }
    }


    [HttpGet("resume/{id}")]
    public ActionResult<dynamic> GetResume(string id)
    {
        try
        {
            dynamic path = $"cv/{id}..pdf";
            var extension = Path.GetExtension(path);
            var content = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(content, "application/octet-stream");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("meetings")]
    public async Task<ActionResult<MeetingDto>> GetMeetings()
    {
        try
        {
            var data = await _repo.GetMeetings();

            var response = new {
                code = 200,
                message = "Success",
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

    [HttpPost("meeting")]
    public async Task<ActionResult> CreateMeeting(MeetingDto payload) {
        try {
            var meeting = await _repo.GetMeeting(payload);

            var pendingMeetings = await _repo.CheckMeetingStatus(payload.ParticipantId);

            if(pendingMeetings.Any()) {
                    var response = new {
                    code = 401,
                    message = "That candidate already has a meetings scheduled"
                };

                return Ok(response);
            }
            else if(meeting.Any()) {

                var response = new {
                    code = 401,
                    message = "There is already a scheduled meeting for that time and date"
                };

                return Ok(response);
            } else {
                var token = await _repo.CreateMeetingToken();
                        var data = await _repo.CreateMeeting(payload, token);

            payload.Link = data.Link;
            payload.MeetingId = data.MeetingId;
            payload.Password = data.Password;

            var mailObj = new EmailDto {
                Firstname = payload.FirstName,
                EmailAddress = payload.Email,
                Subject = $"Invitation for an Interactive Session:{payload.JobTitle}",
                HasFile = "No",
                Body = HTMLHelper.Interview(payload),
            };

            var stageUpdate = new UpdateRole {
                Id = payload.Id,
                Stage = "3"
            };

            await _repo.UpdateStage(stageUpdate);
            
            payload.Completed = "false";

            await _repo.StoreSessionInfo(data);

            CredentialsObj cred = await _repo.GetCredentials();

            await _repo.SendMail(mailObj, cred);
        
            var response = new {
                code = 200,
                message = "Success",
                data
            };

            return Ok(response);
            }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("stage")]
    public async Task<ActionResult> GetCandidateByStage(StageDto payload) {
        try {
            var data = await _repo.GetCandidateByStage(payload);

            var response = new {
                code = 200,
                message = "Successful",
                data
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("metrics")]
    public async Task<ActionResult> GetMetrics() {
        try {
            var data = await _repo.GetMetrics();

            var response = new {
                code = 200,
                data
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var error = new {
                code = 500,
                message = "Unnable to process this request"
            };
            return StatusCode(500, error);
        }
    }

    [HttpPost("user")]
    public async Task<ActionResult> CreateUser(BasicInfo payload) {
        try {

            var duplicate = await _repo.CheckEmail(payload.Email);

            if(!duplicate.Any()) {
                var uuid = guid;
            payload.Id = uuid.ToString();
            payload.Password = BC.HashPassword(payload.Password);
            payload.EmailValid = "False";

            var data = await _repo.CreateUser(payload);

            var claims = new List<Claim>
        {
            new Claim("email", payload.Email),
            new Claim(ClaimTypes.Role, "Administrator"),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);

            CredentialsObj cred = await _repo.GetCredentials();

            var emailAddress = payload.Email;
            
            payload.Email = AEShandler.Encrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));


                    var mailObj = new EmailDto {
                        Firstname = payload.FirstName,
                        EmailAddress = emailAddress,
                        Subject = "Please confirm your email address",
                        HasFile = "No",
                        Body = HTMLHelper.VerifyEmail(payload),
                    };
    
                    var mail = await _repo.SendMail(mailObj, cred);

            var response = new {
                code = 200,
                message = "Successfull",
                data
            };

            return Ok(response);
            }
            else
                {
                    var response = new
                    {
                        code = 401,
                        message = "Applicant already exists",
                    };
                    return Ok(response);
                }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Unsuccessfull"
            };

            return StatusCode(500, response);
        }
    }
    
    [HttpPost("signin")]
    public async Task<ActionResult> SignIn(SignInDto payload) {
        try {
            var data = await _repo.GetBasicInfo(payload?.Email);

            // Console.WriteLine(BC.Verify(payload.Password, data.First().Password));

            // Console.WriteLine(payload.Password);

            // Console.WriteLine(data.First().Password);
            
            if(data.Any() && BC.Verify(payload.Password, data.First().Password) == true) {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, payload.Email),
                    new Claim(ClaimTypes.Name, data.First().Id),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                        HttpContext.Response.Cookies.Append(
                      "cookieKey",
                      "cookieValue",
                      new CookieOptions { IsEssential = true }
                  );
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    Path = "/",
                    Secure = false,
                    HttpOnly = false,
                };

                Response.Cookies.Append("SomeCookie", "SomeValue", cookieOptions);

                var response = new {
                code = 200,
                message = "Successfull",
                data = data.First()
            };

            return Ok(response);
            }
            else if(!data.Any()) {

            var response = new {
                code = 404,
                message = "That user does not exist",
            };

            return Ok(response);
            }
            else if(data.Any() && BC.Verify(payload.Password, data.First().Password) == false) {
                var response = new {
                code = 404,
                message = "That password is incorrect",
            };

            return Ok(response);
            }
            else {
                var response = new {
                code = 404,
                message = "Unnable to process your request",
            };

            return Ok(response);
            }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }
    [HttpPost("admin/auth")]
    public async Task<dynamic> AdminAuth(AdminDto payload) {
        try {
            var data = await _repo.AdminAuth(payload);

            return Ok(data);
        }
        catch(Exception e) {

            Console.Write(e.Message);
            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };
            return StatusCode(500, response);
        }
    }
    [HttpPost("validate")]
    public async Task<ActionResult> ValidateEmail(CandidateEmailDto payload) {
        try {

            var decryptedEmail = AEShandler.Decrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

            var user = await _repo.GetBasicInfo(decryptedEmail);
            
            if(user.Any()) {
                var data = await _repo.ConfirmEmail(decryptedEmail);

                var response = new {
                    code = 200,
                    message = "Email verified successfully",
                };

                return Ok(response);
            }
            else {
                var response = new {
                code = 400,
                message = "User does not exist",
            };

            return Ok(response);
            }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Unnable to process this request",
            };

            return StatusCode(500, response);
        }
    }
    [HttpPost("signout")]
    public async Task<ActionResult> SignUserOut()
    {
        try {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            var response = new {
                code = 200,
                message = "Successfuly signe out"
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("comment")]
    public async Task<ActionResult> Comment(CommentDto payload) {
        try {
            payload.Date = DateTime.Now;

            await _repo.CreateComment(payload);
            
            var response = new {
                code = 200,
                message = "Successful"
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.Write(e.Message);
            var response = new {
                code = 500,
                message = "An error occured processing this request"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("getcomments")]
    public async Task<ActionResult> GetComments(CandidateDto payload) {
        try {
            var data = await _repo.GetComments(payload.Id);
            
            var response = new {
                code = 200,
                message = "Successful",
                data
            };

            return Ok(response);
        }
        catch(Exception e) {
            Console.Write(e.Message);
            var response = new {
                code = 500,
                message = "An error occured processing this request"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("teamsMeeting")]
    public async Task<ActionResult> CreateTeamsMeeting(AdminDto payload) {
        try {
            var data = await _repo.CreateTeamsMeeting();

            return Ok(data);
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            var response = new {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    [HttpPost("pdf")]
    public ActionResult ParsePdf(HireDto payload)
    {
        try
        {
            var data = _repo.SendOfferMail(payload);

            return Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }
}