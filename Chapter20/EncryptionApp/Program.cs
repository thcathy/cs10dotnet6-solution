using My.Shared;
using System.Security.Cryptography;

using static System.Console;

Write("Enter a message to encrypt: ");
string? message = ReadLine();

Write("Enter a password: ");
string? password = ReadLine();

if (password is null || message is null) {
    WriteLine("message or password is null");
    return;
}

string cipherText = Protector.Encrypt(message, password);
WriteLine($"Encrypted text: {cipherText}");

Write("Enter the password: ");
string? password2 = ReadLine();

if (password2 is null) {
    WriteLine("no password to decrypt");
    return;
}

try {
    string clearText = Protector.Decrypt(cipherText, password2);
    WriteLine($"decrypted text: {clearText}");
} catch (CryptographicException ex) {
    WriteLine($"wrong password la \n details: {ex.Message}");
} catch (Exception e) {
    WriteLine($"exception: {e.GetType().Name}, {e.Message}");
}

