using My.Shared;
using static System.Console;

Write("enter text to sign: ");
string? data = ReadLine();

string signature = Protector.GenerateSignature(data);

WriteLine($"Signature: {signature}");
WriteLine("public key is:");
WriteLine(Protector.PublicKey);

verify(data, signature);
if (Protector.ValidateSignature(data, signature)) {
        WriteLine("signature correct lor");
    } else {
        WriteLine($"invalid signature: {signature}");
    }

string fakeSignature = signature.Replace(signature[0], signature[0] == 'X' ? 'Y' : 'X');
verify(data, fakeSignature);
if (Protector.ValidateSignature(data, fakeSignature)) {
        WriteLine("signature correct lor");
    } else {
        WriteLine($"invalid signature: {fakeSignature}");
    }


void verify(string? str, string sign) {
    
}