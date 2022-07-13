using System.Collections.ObjectModel;
using My.Shared;
using static System.Console;

Write("how big do you want the key in bytes? ");
string? size = ReadLine();

byte[] key = Protector.GetRandomKeyOrIV(int.Parse(size));

WriteLine($"key as byte array:");
for (int b = 0; b < key.Length; b++) {
    Write($"{key[b]:x2} ");
    if (((b+1) % 16) == 0) WriteLine();
}
WriteLine();