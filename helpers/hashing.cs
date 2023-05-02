using System.Security.Cryptography;
using System.Text;

namespace Hashing;

public static class Password
{
    const int keySize = 32;
    const int iterations = 350000;
    static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

    public static string Hash(string password, out byte[] salt)
    {

        salt = RandomNumberGenerator.GetBytes(keySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);

        return Convert.ToHexString(hash);
    }

    public static bool Verify(string password, string hash, byte[] salt)
{
    var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
    return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
}
}