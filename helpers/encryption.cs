using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoHelper;

public static class CryptoService
{
    public static string Encrypt(string plainText, string key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                return Convert.ToBase64String(aesAlg.IV) + Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

public static string Decrypt(string encryptedData, string key)
    {
        byte[] combinedData = Convert.FromBase64String(encryptedData);
        byte[] iv = new byte[16];
        byte[] cipherText = new byte[combinedData.Length - 16];

        Array.Copy(combinedData, iv, 16);
        Array.Copy(combinedData, 16, cipherText, 0, cipherText.Length);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
       public static string DecryptStringAES(string cipherText) {
        var keybytes = Encoding.UTF8.GetBytes("4512631236589784");
        var iv = Encoding.UTF8.GetBytes("4512631236589784");

        var encrypted = Convert.FromBase64String(cipherText);
        var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
        return decriptedFromJavascript;
    }

       private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv) {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0) {
            throw new ArgumentNullException("cipherText");
        }
        if (key == null || key.Length <= 0) {
            throw new ArgumentNullException("key");
        }
        if (iv == null || iv.Length <= 0) {
            throw new ArgumentNullException("key");
        }

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        using(var rijAlg = new RijndaelManaged()) {
            //Settings
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrytor to perform the stream transform.
            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            try {
                // Create the streams used for decryption.
                using(var msDecrypt = new MemoryStream(cipherText)) {
                    using(var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {

                        using(var srDecrypt = new StreamReader(csDecrypt)) {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();

                        }

                    }
                }
            } catch {
                plaintext = "keyError";
            }
        }

        return plaintext;
    }
    }

