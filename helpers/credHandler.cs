using Candidate.Models;
using AES;
using Microsoft.Data.SqlClient;
using Dapper;
using Credentials.Models;
using Newtonsoft.Json.Linq;

namespace CredentialsHandler;

public class CredHandler {
    public static string id { get; set; } = "72203447f8292549e12680877545";
    private readonly IConfiguration _config;
    public CredHandler(IConfiguration config)
    {
        this._config = config;
    }

    public async Task<dynamic> MakeContract() {

        HttpClient client = new();
        
            using HttpResponseMessage response = await client.GetAsync("http://10.0.0.184:8015/03a3b2c6f7d8e1c4_0a");

            var credentials = "";

            if(response.Headers.TryGetValues("x-lapo-eve-proc", out IEnumerable<string> resHeaders)) {
                credentials = resHeaders.FirstOrDefault();
            }

            return credentials.Split("~");
    }

    public async Task Renew() {
            try {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            
            HttpClient client = new();

            var credentials = await connection.QueryAsync<CredentialsObj>("SELECT token, aeskey, aesiv FROM tokens WHERE id = '100'");

            var token = credentials.FirstOrDefault().Token;

            var aesKey = credentials.FirstOrDefault().AesKey;

            var aesIv = credentials.FirstOrDefault().AesIv;

            client.DefaultRequestHeaders.Add("x-lapo-eve-proc", token + id);

            using HttpResponseMessage response = await client.GetAsync("http://10.0.0.184:8023/generateuserkey");

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using StreamWriter outputFile = new("C:/Users/LAPO Mfb/Desktop/tokenlogs/cred.txt", true);

            await outputFile.WriteAsync(jsonResponse);

            var jsonObject = JObject.Parse(jsonResponse);

            byte[] stringBytes = Convert.FromHexString(jsonObject.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, aesKey, aesIv);

            var decryptedJson = JObject.Parse(decrypted);

            var cred = new CredentialsObj {
                AesIv = decryptedJson.Value<string>("aesIv"),
                AesKey = decryptedJson.Value<string>("aesKey"),
                Token = decryptedJson.Value<string>("access_token"),
            };

            if(cred != null) {
                Console.WriteLine("got the credentials");
                await connection.ExecuteAsync("UPDATE tokens SET token = @Token, aeskey = @AesKey, aesiv = @AesIv WHERE id = '100'", cred);
            }
        }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
    }
}