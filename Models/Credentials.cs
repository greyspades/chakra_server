using System.Security.Cryptography.Pkcs;

namespace Credentials.Models;


public class CredentialsObj {
    public string? Token { get; set; }
    public string? AesKey { get; set; }
    public string? AesIv { get; set; }
}

public class CredentialsRes {
    public string? AesKey { get; set; }
    public string? AesIv { get; set; }
    // public string? Access_Token { get; set; }
    // public dynamic? Token_exp { get; set; }

}