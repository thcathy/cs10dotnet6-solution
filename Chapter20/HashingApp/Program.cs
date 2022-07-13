using My.Shared;

using static System.Console;

WriteLine("Register Alice with Pa$$w0rd:");
User alice = Protector.Register("Alice", "Pa$$w0rd");

WriteLine($"  Name: {alice.Name}");
WriteLine($"  Salt: {alice.Salt}");
WriteLine($"  Password: {alice.SaltHashedPassword}");
WriteLine();

Write("enter a new user to register: ");
string? username = ReadLine();

Write($"enter password for {username}: ");
string? password = ReadLine();

if (username is null || password is null) {
    WriteLine("no input wor");
    return;
}

WriteLine("register a new user:");
User user1 = Protector.Register(username, password);
WriteLine($"  Name: {user1.Name}");
WriteLine($"  Salt: {user1.Salt}");
WriteLine($"  Password: {user1.SaltHashedPassword}");
WriteLine();

bool correctPassword = false;
while (!correctPassword) {
    Write("enter login username: ");
    string? loginUsername = ReadLine();

    Write("enter password: ");
    string? loginPassword = ReadLine();

    if (loginUsername is null || loginPassword is null) {
        WriteLine("no input wor. Continue...");
        continue;
    }

    correctPassword = Protector.CheckPassword(loginUsername, loginPassword);
    if (correctPassword) {
        WriteLine($"correct! {username} verified");
    } else {
        WriteLine("invalid password");
    }
}