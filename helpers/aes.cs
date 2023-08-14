using System.Security.Cryptography.Xml;
using System.Text.Json;
using NETCore.Encrypt;

namespace AES;

public static class AEShandler {
    public static string Encrypt(string content, string key, string iv)
    {
            var encrypted = EncryptProvider.AESEncrypt(content, key, iv);

            return encrypted;
    }

    public static string Decrypt(string content, string key, string iv) {

        var decrypted = EncryptProvider.AESDecrypt(content, key, iv);

        return decrypted;
    }

     public static string EncryptResponse(dynamic content) {
      return EncryptProvider.AESEncrypt(JsonSerializer.Serialize(content), "yy7a1^.^^j_ii^c2^5^ho_@.9^d7bi^.", "h!!_2bz^(@?yyq!.");
    }
}