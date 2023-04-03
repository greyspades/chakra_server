using Candidate.Models;
using AES;
using Microsoft.Data.SqlClient;
using Dapper;

namespace CredentialsHandler;

public class Credentials {
    public static string defaultToken { get; set; } = "c4307a90c6fe0ee9a5a65c5c491b096303afc9885059df0c5844d9692497bdb38981c5d78951725f008d7a846aa0e3a0594fceed2c44c371047ffcaf5b4ca359";
    public static string defaultKey { get; set; } = "ci.@!_^^2!auaa!h0!)4a2h!1a^1(_r!";
    public static string defaultIv { get; set; } = ".1_r@l__3_7_)n!(";
    public static string id { get; set; } = "72203447f8292549e12680877545";

    public static string? token { get; set; }

    public static string? key { get; set; }

    public static string? iv { get; set; }
    private readonly IConfiguration _config;
    public Credentials(IConfiguration config)
    {
        this._config = config;
    }

    public async Task Renew() {
            try {
                HttpClient client = new();
        
            client.DefaultRequestHeaders.Add("x-lapo-eve-proc", defaultToken + id);
            
            Console.WriteLine(client.DefaultRequestHeaders);

            using HttpResponseMessage response = await client.GetAsync("http://10.0.0.184:8023/generateuserkey");

            var jsonResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine(jsonResponse);

            using StreamWriter outputFile = new("C:/Users/LAPO Mfb/Desktop/tokenlogs/cred.txt");
            await outputFile.WriteAsync(jsonResponse);

            byte[] stringBytes = Convert.FromHexString(jsonResponse);

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, defaultKey, defaultIv);

            Console.WriteLine(decrypted);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var data = await connection.ExecuteAsync("UPDATE tokens SET token = '3296r734r' WHERE id = '100'");
            Console.WriteLine(data);

        }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
    }
}