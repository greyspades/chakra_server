using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Admin.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;


namespace Admin.Controllers;

[Route("api/[Controller]")]
[ApiController]

public class AdminController : ControllerBase
{
    private readonly IConfiguration _config;
    Guid guid = Guid.NewGuid();
    public AdminController(IConfiguration config)
    {
        this._config = config;
    }

    [HttpPost("New")]
    public async Task<ActionResult> AddAdmin(AdminModel payload)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            payload.Id = guid.ToString();

            var data = await connection.ExecuteAsync("INSERT into Admins (username, password, id) VALUES (@Username, @Password, @Id)", payload);

            var response = new
            {
                code = 200,
                message = "Successful",
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

    [HttpGet]
    public async Task<ActionResult> GetAdmins()
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var data = await connection.QueryAsync<AdminModel>("SELECT * from Admins");

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

    [HttpPost("Auth")]
    public async Task<ActionResult> AuthenticateAdmin(AdminModel payload)
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var data = await connection.QueryAsync<AdminModel>("SELECT * from Admins where username = @Username AND password = @Password", payload);

            if (data.Any())
            {
                var response = new
                {
                    code = 200,
                    message = "Successful",
                    data
                };

                return Ok(response);
            }
            else
            {
                var response = new
                {
                    code = 401,
                    message = "Credentials not Correct",
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