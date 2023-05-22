using Candidate.Models;
using AES;
using Microsoft.Data.SqlClient;
using Dapper;
using Credentials.Models;
using Newtonsoft.Json.Linq;
using System.Data;

namespace CredentialsHandler;

public class CredHandler
{
    // public static string id { get; set; } = ;
    private readonly IConfiguration _config;
    public CredHandler(IConfiguration config)
    {
        this._config = config;
    }

    public async Task<dynamic> MakeContract()
    {

        HttpClient client = new();

        using HttpResponseMessage response = await client.GetAsync(_config.GetValue<string>("E360:contract"));

        var credentials = "";

        if (response.Headers.TryGetValues("x-lapo-eve-proc", out IEnumerable<string> resHeaders))
        {
            credentials = resHeaders.FirstOrDefault();
        }

        return credentials.Split("~");
    }

    public async Task Renew()
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            HttpClient client = new();

            var credentials = await connection.QueryAsync<CredentialsObj>("Get_credentials", commandType: CommandType.StoredProcedure);

            var token = credentials.FirstOrDefault().Token;

            var aesKey = credentials.FirstOrDefault().AesKey;

            var aesIv = credentials.FirstOrDefault().AesIv;

            client.DefaultRequestHeaders.Add("x-lapo-eve-proc", token + _config.GetValue<string>("LASM_cred:Id"));

            using HttpResponseMessage response = await client.GetAsync(_config.GetValue<string>("LASM_cred:User_key_url"));

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using StreamWriter outputFile = new("C:/Users/LAPO Mfb/Desktop/tokenlogs/cred.txt", true);

            await outputFile.WriteAsync(jsonResponse);

            var jsonObject = JObject.Parse(jsonResponse);

            byte[] stringBytes = Convert.FromHexString(jsonObject.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, aesKey, aesIv);

            var decryptedJson = JObject.Parse(decrypted);

            var cred = new CredentialsObj
            {
                AesIv = decryptedJson.Value<string>("aesIv"),
                AesKey = decryptedJson.Value<string>("aesKey"),
                Token = decryptedJson.Value<string>("access_token"),
            };

            if (cred != null)
            {
                Console.WriteLine("got the credentials");
                await connection.ExecuteAsync("Store_credentials", cred, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}