namespace My.Shared;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using static System.Console;
using static System.Convert;

public static class Protector
{
    private static readonly byte[] salt = Encoding.Unicode.GetBytes("7BANANAS");

    private static readonly int iterations = 100_100;

    private static Dictionary<string, User> Users = new();

    public static string? PublicKey;

    public static string GenerateSignature(string data) {
        byte[] dataBytes = Encoding.Unicode.GetBytes(data);
        SHA256 sha = SHA256.Create();
        byte[] hashedData = sha.ComputeHash(dataBytes);
        RSA rsa = RSA.Create();

        PublicKey = rsa.ToXmlString(false);
        return ToBase64String(rsa.SignHash(hashedData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
    }

    public static bool ValidateSignature(string data, string signature) {
        if (PublicKey is null)
        {
            WriteLine("ng ha ar far");
            return false;
        }

        byte[] dataBytes = Encoding.Unicode.GetBytes(data);
        SHA256 sha = SHA256.Create();
        byte[] hashedData = sha.ComputeHash(dataBytes);
        byte[] signatureBytes = FromBase64String(signature);
        RSA rsa = RSA.Create();
        rsa.FromXmlString(PublicKey);
        return rsa.VerifyHash(hashedData, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static string Encrypt(string plainText, string password) {
        byte[] encryptedBytes;
        byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

        using (Aes aes = Aes.Create()) {
            Stopwatch timer = Stopwatch.StartNew();

            using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations)) {
                aes.Key = pbkdf2.GetBytes(32);
                aes.IV = pbkdf2.GetBytes(16);
            }
            timer.Stop();
            WriteLine($"{timer.ElapsedMilliseconds:N0} ms to generate key and IV using {iterations}");

            using (MemoryStream ms = new()) {
                using (ICryptoTransform transformer = aes.CreateEncryptor()) {
                    using (CryptoStream cs = new(ms, transformer, CryptoStreamMode.Write)) {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }
                }
                encryptedBytes = ms.ToArray();
            }
        }
        return ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string cipherText, string password)
    {
        byte[] plainBytes;
        byte[] cryptoBytes = FromBase64String(cipherText);

        using (Aes aes = Aes.Create()) {
            using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations)) {
                aes.Key = pbkdf2.GetBytes(32);
                aes.IV = pbkdf2.GetBytes(16);
            }

            using (MemoryStream ms = new()) {
                using (ICryptoTransform transformer = aes.CreateDecryptor()) {
                    using (CryptoStream cs = new(ms, transformer, CryptoStreamMode.Write)) {
                        cs.Write(cryptoBytes, 0, cryptoBytes.Length);
                    }
                }
                plainBytes = ms.ToArray();
            }
        }
        return Encoding.Unicode.GetString(plainBytes);
    }

    public static User Register(string username, string password, string[]? roles = null) {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        string saltText = ToBase64String(saltBytes);

        string saltHashedPassword = SaltAndHashPassword(password, saltText);

        User user = new(username, saltText, saltHashedPassword, roles);
        Users.Add(username, user);
        return user;
    }

    public static bool CheckPassword(string username, string password) {
        User? u = Users[username];
        if (u is null) {
            return false;
        }
        return CheckPassword(password, u.Salt, u.SaltHashedPassword);
    }

    private static bool CheckPassword(string password, string salt, string saltHashedPassword) {
        string hashedInput = SaltAndHashPassword(password, salt);
        return saltHashedPassword == hashedInput;
    }

    private static string SaltAndHashPassword(string password, string salt) {
        using (SHA256 sha = SHA256.Create()) {
            string saltedPassword = password + salt;
            return ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));
        }
    }

    public static byte[] GetRandomKeyOrIV(int size) {
        RandomNumberGenerator r = RandomNumberGenerator.Create();
        byte[] data = new byte[size];
        r.GetBytes(data);
        return data;
    }

    public static void LogIn(string username, string password) {
        if (CheckPassword(username, password)) {
            GenericIdentity gi = new(username, "MyAuth");
            GenericPrincipal gp = new(gi, Users[username].Roles);
            Thread.CurrentPrincipal = gp;
        }
    }
}
