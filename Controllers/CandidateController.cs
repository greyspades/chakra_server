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
using JobRole.Interface;
using Newtonsoft.Json.Linq;

namespace Candidate.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
    Guid guid = Guid.NewGuid();
    private readonly ICandidateRepository _repo;
    private readonly IJobRoleRepository _jobRepo;
    public IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    // private readonly IMongoCollection<ResumeModel> _collection;
    // private readonly IMongoDatabase _mongoDb;
    private static readonly HttpClient client = new();
    public CandidateController(IWebHostEnvironment environment, ICandidateRepository repo, IJobRoleRepository jobRepo, IConfiguration config)
    {
        this._repo = repo;
        this._env = environment;
        this._config = config;
        this._jobRepo = jobRepo;
        // var mongoClient = new MongoClient(
        //     dbSettings.Value.ConnectionString);

        // var mongoDatabase = mongoClient.GetDatabase(
        //     dbSettings.Value.DatabaseName);

        // _collection = mongoDatabase.GetCollection<ResumeModel>(
        //     dbSettings.Value.CollectionName);
        // _mongoDb = mongoDatabase;
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidates(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<GetCandidatesDto>();

            var data = await _repo.GetCandidates();

            var count = payload.Page * 10;

            var slicedCandidates = data.Skip((int)count!).Take((int)payload.Take!);

            var response = new
            {
                code = 200,
                data = AEShandler.EncryptResponse(slicedCandidates),
                count = data.Count()
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var response = new
            {
                code = 500,
                message = "Unnable to proceed with your request"
            };
            return StatusCode(500, response);
        }
    }
    [Authorize]
    [HttpPost("id")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateById(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<CandidateDto>();

            var data = await _repo.GetCandidateById(payload.Id);

            var sample = new
            {
                code = 200,
                message = "Successful",
                data = AEShandler.EncryptResponse(data)
            };

            return Ok(sample);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode(500, e.Message);
        }
    }

    // [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult> CreateCandidate([FromForm] CandidateModel payload)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // var payload = jObject.Data;
                // Console.WriteLine(payload.FirstName);

                var dateDifference = (DateTime.Now.Date - payload?.Dob.Value)?.TotalDays;

                var info = await _repo.GetBasicInfo(payload?.Email);

                var applications = await _repo.GetApplicationsByMail(payload.Email);

                for (int i = 0; i < applications.Count(); i++)
                {
                    if (applications.ElementAt(i).RoleId == payload.RoleId)
                    {
                        var response = new
                        {
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
                if (
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
                    payload.MaritalStatus = info.First().MaritalStatus;
                    payload.Address = info.First().Address;
                    // payload.State = info.First().State;
                    // payload.Lga = info.First().Lga;

                    var data = await _repo.CreateCandidate(payload);

                    CredentialsObj cred = await _repo.GetCredentials();

                    var mailObj = new EmailDto
                    {
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
                // return Ok("fw");
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

    [Authorize]
    [HttpGet("token")]
    public async Task<ActionResult> CreateMeetingToken()
    {
        try
        {
            var data = await _repo.CreateMeetingToken();

            return Ok(data);
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpPost("mail")]
    public async Task<ActionResult<string>> SendMail(JObject jObject)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var payload = jObject.ToObject<EmailDto>();

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
        else
        {
            return StatusCode(400, "Invalid request body");
        }
    }

    [Authorize]
    [HttpPost("skills")]
    public async Task<ActionResult<List<string>>> GetSkills(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<CandidateDto>();

                var data = await _repo.GetSkills(payload.Id);

                var response = new
                {
                    code = 200,
                    data = AEShandler.EncryptResponse(data)
                };

                return Ok(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }

    [Authorize]
    [HttpPost("flag")]
    public async Task<ActionResult<dynamic>> FlagCandidate(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<FlagCandidateDto>();

                var data = await _repo.FlagCandidate(payload);

                if (payload.Flag == "notFit")
                {
                    CredentialsObj cred = await _repo.GetCredentials();

                    var candidate = await _repo.GetCandidateById(payload.Id);

                    var candidateData = candidate.First();

                    await _repo.CancelApplication(payload.Id);

                    var mailObj = new EmailDto
                    {
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
    }

    [Authorize]
    [HttpPost("flag/candidates")]
    public async Task<ActionResult<List<CandidateModel>>> GetByFlag(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<CandidateByFlagDto>();

                var data = await _repo.GetByFlag(payload);

                var response = new
                {
                    code = 200,
                    message = "Successfull",
                    data = AEShandler.EncryptResponse(data),
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }

    [Authorize]
    [HttpPost("skills/candidates")]
    public async Task<ActionResult<List<string>>> GetCandidateBySkill(JObject jObject)
    {
        try
        {
            var payload = jObject.ToObject<SkillsInput>();

            var data = await _repo.GetCandidateBySkills(payload);

            var response = new
            {
                code = 200,
                data = AEShandler.EncryptResponse(data)
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpPost("hire")]
    public async Task<ActionResult> HireCandidate(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<HireDto>();

                var can = await _repo.GetCandidateById(payload?.Id);

                var canData = can.First();

                // Console.WriteLine(canData.FirstName);

                var basic = await _repo.GetBasicInfo(canData.Email);

                var basicInfo = basic.First();

                if (canData.TempId == null)
                {
                    payload.Address = basicInfo.Address;
                    payload.FirstName = canData.FirstName;
                    payload.LastName = canData.LastName;
                    payload.Position = canData.JobName;
                    payload.HireDate = DateTime.Now;

                    _repo.CreateOfferMail(payload);

                    if (payload.SendMail == true)
                    {
                        var mailObj = new EmailDto
                        {
                            Firstname = canData.FirstName,
                            EmailAddress = canData.Email,
                            Subject = $"Your application for {canData.JobName} at LAPO Microfinance Bank",
                            HasFile = "Yes",
                            Body = HTMLHelper.Acceptance(canData)
                        };

                        CredentialsObj cred = await _repo.GetCredentials();

                        var mail = await _repo.SendMail(mailObj, cred);

                        using StreamWriter outputFile = new("tokenlogs.txt", true);

                        await outputFile.WriteAsync(mail);
                    }

                    await _repo.HireCandidate(payload);

                    var response = new
                    {
                        code = 200,
                        message = "Candidate has been hired successfully",
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        code = 401,
                        message = "Candidate has already been hired",
                    };

                    return Ok(response);
                }

                //  var response = new
                // {
                //     code = 401,
                //     message = "Candidate has already been hired",
                // };

                // return Ok(response);
                // }
            }
            catch (Exception e)
            {
                Console.WriteLine(value: e.Message);

                using StreamWriter outputFile = new("tokenlogs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var result = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, result);
            }
        }
    }

    // [HttpPost("/upload")]
    // public async Task<ActionResult> UploadCv(byte[] cv)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return StatusCode(400, "Invalid request body");
    //     }
    //     else
    //     {
    //         try
    //         {
    //             // Console.WriteLine(cv.FileName);
    //             if (cv != null)
    //             {
    //                 var data = await _repo.ParseCvAsync(cv, guid);

    //                 var response = new
    //                 {
    //                     code = 200,
    //                     message = "success",
    //                     data,
    //                     // cvMeta = cvMetaData
    //                 };

    //                 return Ok(response);
    //             }
    //             return Ok(new { code = 500, message = "Unsuccessful" });
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine(e.Message);
    //             return StatusCode(500, e.Message);
    //         }
    //     }
    // }
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

    [Authorize]
    [HttpPost("/stage")]
    public async Task<ActionResult> UpdateStage(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<UpdateRole>();

                var candidate = await _repo.GetCandidateById(payload.Id);
                if (candidate.First().Stage != payload.Stage)
                {
                    var data = await _repo.UpdateStage(payload);

                    var response = new
                    {
                        code = 200,
                        message = "Successfully moved to the next stage"
                    };

                    return Ok(response);
                }
                else
                {
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
    }

    [Authorize]
    [HttpPost("role")]
    public async Task<ActionResult<List<CandidateModel>>> GetCandidateByRole(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<GetCandidatesDto>();

                IEnumerable<CandidateModel> data;
                if (payload.Id != null && payload.Id != "")
                {
                    data = await _repo.GetCandidatesByRole(payload.Id);
                }
                else
                {
                    data = await _repo.GetCandidates();
                }
                var count = payload.Page * 10;

                IEnumerable<CandidateModel> filteredData;

                if (payload.Filter == "status")
                {
                    filteredData = data.Where((CandidateModel item) => item.Status == payload.FilterValue);
                }
                else if (payload.Filter == "stage")
                {
                    filteredData = data.Where((CandidateModel item) => item.Stage == payload.FilterValue);
                }
                else if (payload.Filter == "date")
                {
                    DateTime startDate = DateTime.Parse(payload.FilterValue.Split(":")[0]);
                    DateTime endDate = DateTime.Parse(payload.FilterValue.Split(":")[1]);
                    filteredData = data.Where((CandidateModel item) => item.ApplDate >= startDate && item.ApplDate <= endDate);
                }
                else
                {
                    filteredData = data;
                }

                var slicedCandidates = filteredData.Skip((int)count!).Take((int)payload.Take!);

                var response = new
                {
                    code = 200,
                    data = AEShandler.EncryptResponse(slicedCandidates),
                    count = filteredData.Count()
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return StatusCode(500, e.Message);
            }
        }
    }

    [Authorize]
    [HttpPost("cancel")]
    public async Task<ActionResult> CancelApplication(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<CancelApplication>();

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
    }

    [Authorize]
    [HttpPost("/status")]
    public async Task<ActionResult<CandidateModel>> GetStatus(JObject jObject)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var payload = jObject.ToObject<GetStatusDto>();

                var info = await _repo.GetBasicInfo(payload?.Email);

                // Console.Write(info.First().LastName);

                if (info.Any())
                {
                    var data = await _repo.GetStatus(payload);


                    var response = new
                    {
                        code = 200,
                        message = "Successful",
                        data = AEShandler.EncryptResponse(data)
                    };
                    return Ok(response);
                }
                else if (info.First() == null)
                {
                    var response = new
                    {
                        code = 400,
                        message = "This Candidate does not exist, please check your email address and try again",

                    };
                    return Ok(response);
                }
                else
                {
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
            return StatusCode(400, "Invalid request body");
        }
    }
    [Authorize]
    [HttpPost("mail_reset_options")]
    public async Task<ActionResult> MailResetOptions(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<CandidateEmailDto>();

                var candidate = await _repo.GetCandidateByMail(payload.Email);

                if (candidate.Any())
                {
                    CredentialsObj cred = await _repo.GetCredentials();

                    // var encryptedEmail = AEShandler.Encrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                    var encryptedEmail = AEShandler.Encrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                    byte[] bytes = Convert.FromBase64String(encryptedEmail);

                    string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                    // Console.WriteLine(encryptedEmail);
                    // Console.WriteLine($"localhost:8089/password_reset?email={encryptedEmail}&token=00727143910");

                    var resetObj = new PasswordResetFields
                    {
                        Email = hexString,
                        FirstName = candidate.First().FirstName
                    };

                    var mailObj = new EmailDto
                    {
                        Firstname = candidate.First().FirstName,
                        EmailAddress = payload.Email,
                        Subject = "Reset your password",
                        HasFile = "No",
                        Body = HTMLHelper.PasswordReset(resetObj),
                    };

                    var mailRes = await _repo.SendMail(mailObj, cred);

                    var response = new
                    {
                        code = 200,
                        message = "Please click the link sent to your email to reset your password",
                        // mail = mailRes
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        code = 404,
                        message = "That Email address does not exist on the system",

                    };

                    return Ok(response);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(value: e.Message);

                using StreamWriter outputFile = new("tokenlogs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var result = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, result);
            }
        }
    }

    [Authorize]
    [HttpPost("reset_password")]
    public async Task<ActionResult> ResetPassword(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<PasswordResetDto>();

                byte[] stringBytes = Convert.FromHexString(payload.Email);

                string bytes = Convert.ToBase64String(stringBytes);

                payload.Email = AEShandler.Decrypt(bytes, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                var user = await _repo.CheckEmail(payload.Email);

                if (user.Any())
                {
                    payload.Password = BC.HashPassword(payload.Password);

                    payload.Id = user.First().Id;

                    await _repo.ResetPassword(payload);

                    var response = new
                    {
                        code = 200,
                        message = "Your password has been updated successfully"
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        code = 404,
                        message = "Sorry that email address does not exist on our records"
                    };

                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(value: e.Message);

                using StreamWriter outputFile = new("tokenlogs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var result = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, result);
            }
        }
    }

    [HttpGet("resume/{id}")]
    public ActionResult<dynamic> GetResume(string id)
    {
        try
        {
            // var id = jObject.ToObject<string>();

            dynamic path = @$"cv/{id}..pdf";

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

    [Authorize]
    [HttpPost("meetings")]
    public async Task<ActionResult<MeetingDto>> GetMeetings(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<GetCandidatesDto>();

                var data = await _repo.GetMeetings(payload.Id);

                var count = payload.Page * 10;

                var slicedMeetings = data.Skip((int)count!).Take((int)payload.Take!);

                var response = new
                {
                    code = 200,
                    message = "Success",
                    count = data.Count(),
                    data = AEShandler.EncryptResponse(slicedMeetings)
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("tokenlogs.txt", true);

                await outputFile.WriteAsync(e.Message);

                return StatusCode(500, e.Message);
            }
        }
    }

    [Authorize]
    [HttpPost("meeting")]
    public async Task<ActionResult> CreateMeeting(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<MeetingDto>();

                var meeting = await _repo.GetMeeting(payload);

                var pendingMeetings = await _repo.CheckMeetingStatus(payload.ParticipantId);

                // if (pendingMeetings.Any())
                // {
                //     var response = new
                //     {
                //         code = 401,
                //         message = "That candidate already has a meetings scheduled"
                //     };

                //     return Ok(response);
                // }
                if (meeting.Any())
                {

                    var response = new
                    {
                        code = 401,
                        message = "There is already a scheduled meeting for that time and date"
                    };

                    return Ok(response);
                }
                else
                {
                    var token = await _repo.CreateMeetingToken();
                    var data = await _repo.CreateMeeting(payload, token);

                    payload.Link = data.Link;
                    payload.MeetingId = data.MeetingId;
                    payload.Password = data.Password;

                    var mailObj = new EmailDto
                    {
                        Firstname = payload.FirstName,
                        EmailAddress = payload.Email,
                        Subject = $"Invitation for an Interactive Session:{payload.JobTitle}",
                        HasFile = "No",
                        Body = HTMLHelper.Interview(payload),
                    };

                    payload.Completed = "false";

                    await _repo.StoreSessionInfo(data);

                    var stageUpdate = new UpdateRole
                    {
                        Id = payload.ParticipantId,
                        Stage = "2"
                    };

                    await _repo.UpdateStage(stageUpdate);

                    CredentialsObj cred = await _repo.GetCredentials();

                    await _repo.SendMail(mailObj, cred);

                    var response = new
                    {
                        code = 200,
                        message = "Success",
                        data = AEShandler.EncryptResponse(data)
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
    }

    [Authorize]
    [HttpPost("stage")]
    public async Task<ActionResult> GetCandidateByStage(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<StageDto>();

                var data = await _repo.GetCandidateByStage(payload);

                var count = payload.Page * 10;

                var slicedCandidates = data.Skip((int)count!).Take((int)payload.Take!);

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    count = data.Count(),
                    data = AEShandler.EncryptResponse(slicedCandidates)
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("tokenlogs.txt", true);
                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };
                return StatusCode(500, response);
            }
        }
    }

    [Authorize]
    [HttpGet("metrics")]
    public async Task<ActionResult> GetMetrics()
    {
        try
        {
            var data = await _repo.GetMetrics();

            var response = new
            {
                code = 200,
                data = AEShandler.EncryptResponse(data)
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var error = new
            {
                code = 500,
                message = "Unnable to process this request"
            };
            return StatusCode(500, error);
        }
    }

    [Authorize]
    [HttpPost("user")]
    public async Task<ActionResult> CreateUser(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid reuest body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<BasicInfo>();

                var duplicate = await _repo.CheckEmail(payload.Email);

                if (!duplicate.Any())
                {
                    var uuid = guid;
                    payload.Id = uuid.ToString();
                    payload.Password = BC.HashPassword(payload.Password);
                    payload.EmailValid = "False";

                    var data = await _repo.CreateUser(payload);

                    CredentialsObj cred = await _repo.GetCredentials();

                    var emailAddress = payload.Email;

                    var encryptedEmail = AEShandler.Encrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                    byte[] bytes = Convert.FromBase64String(encryptedEmail);

                    string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                    payload.Email = hexString;

                    // Console.WriteLine(hexString);

                    var mailObj = new EmailDto
                    {
                        Firstname = payload.FirstName,
                        EmailAddress = emailAddress,
                        Subject = "Please confirm your email address",
                        HasFile = "No",
                        Body = HTMLHelper.VerifyEmail(payload),
                    };

                    var mail = await _repo.SendMail(mailObj, cred);

                    // Console.WriteLine(mail);

                    var response = new
                    {
                        code = 200,
                        message = "Successfull",
                        data = AEShandler.EncryptResponse(data)
                    };

                    return Ok(response);
                }
                // {
                //     Console.WriteLine("gets to the block");
                //     var response = new
                //     {
                //         code = 401,
                //         message = "damn son",
                //     };
                //     return Ok(response);
                // }
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                var response = new
                {
                    code = 500,
                    message = "Unsuccessfull"
                };

                return StatusCode(500, response);
            }
        }
    }

    [HttpPost("signin")]
    public async Task<ActionResult> SignIn(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<SignInDto>();

                var data = await _repo.GetBasicInfo(payload?.Email);

                if (data.Any() && BC.Verify(payload.Password, data.First().Password) == true)
                {
                    data.First().Password = null;

                    var response = new
                    {
                        code = 200,
                        message = "Successfull",
                        data = AEShandler.EncryptResponse(data.First())
                    };

                    return Ok(response);
                }
                else if (!data.Any())
                {

                    var response = new
                    {
                        code = 404,
                        message = "Invalid email/password",
                    };

                    return Ok(response);
                }
                else if (data.Any() && BC.Verify(payload.Password, data.First().Password) == false)
                {
                    var response = new
                    {
                        code = 404,
                        message = "Invalid email/password",
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        code = 404,
                        message = "Unnable to process your request",
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
                    message = "Unnable to process your request"
                };

                return StatusCode(500, response);
            }
        }
    }

    [Authorize]
    [HttpPost("admin/auth")]
    public async Task<dynamic> AdminAuth(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<AdminDto>();

                var data = await _repo.AdminAuth(payload);

                // Console.WriteLine(data);
                var response = new
                {
                    code = 200,
                    data = AEShandler.EncryptResponse(data)
                };

                return Ok(response);
            }
            catch (Exception e)
            {

                Console.Write(e.Message);
                var response = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };
                return StatusCode(500, response);
            }
        }
    }

    [Authorize]
    [HttpPost("validate")]
    public async Task<ActionResult> ValidateEmail(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                 var payload = jObject.ToObject<CandidateEmailDto>();

                byte[] stringBytes = Convert.FromHexString(payload.Email);

                string bytes = Convert.ToBase64String(stringBytes);

                var decryptedEmail = AEShandler.Decrypt(bytes, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                // var decryptedEmail = AEShandler.Decrypt(payload.Email, _config.GetValue<string>("Encryption:Key"), _config.GetValue<string>("Encryption:Iv"));

                var user = await _repo.GetBasicInfo(decryptedEmail);

                if (user.Any())
                {
                    var data = await _repo.ConfirmEmail(decryptedEmail);

                    var response = new
                    {
                        code = 200,
                        message = "Email verified successfully",
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        code = 400,
                        message = "User does not exist",
                    };

                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(value: e.Message);

                using StreamWriter outputFile = new("tokenlogs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var result = new
                {
                    code = 500,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, result);
            }
        }
    }

    [Authorize]
    [HttpPost("signout")]
    public async Task<ActionResult> SignUserOut()
    {
        try
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            var response = new
            {
                code = 200,
                message = "Successfuly signe out"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var response = new
            {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpPost("comment")]
    public async Task<ActionResult> Comment(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                 var payload = jObject.ToObject<CommentDto>();

                payload.Date = DateTime.Now;

                await _repo.CreateComment(payload);

                var response = new
                {
                    code = 200,
                    message = "Successful"
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                var response = new
                {
                    code = 500,
                    message = "An error occured processing this request"
                };

                return StatusCode(500, response);
            }
        }
    }

    [Authorize]
    [HttpPost("getcomments")]
    public async Task<ActionResult> GetComments(JObject jObject)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(400, "Invalid request body");
        }
        else
        {
            try
            {
                var payload = jObject.ToObject<CandidateDto>();

                var data = await _repo.GetComments(payload.Id);

                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data = AEShandler.EncryptResponse(data)
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                var response = new
                {
                    code = 500,
                    message = "An error occured processing this request"
                };

                return StatusCode(500, response);
            }
        }
    }

    [Authorize]
    [HttpPost("teamsMeeting")]
    public async Task<ActionResult> CreateTeamsMeeting(AdminDto payload)
    {
        try
        {
            var data = await _repo.CreateTeamsMeeting();

            return Ok(AEShandler.EncryptResponse(data));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var response = new
            {
                code = 500,
                message = "Unnable to process your request"
            };

            return StatusCode(500, response);
        }
    }

    // [HttpPost("pdf")]
    // public ActionResult ParsePdf(HireDto payload)
    // {
    //     try
    //     {
    //         var data = _repo.SendOfferMail(payload);

    //         return Ok(data);
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message);

    //         return StatusCode(500, e.Message);
    //     }
    // }
}