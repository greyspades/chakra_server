using System.Security.Cryptography.Xml;
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
}