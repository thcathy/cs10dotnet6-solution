using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using My.Shared;
using static System.Console;

Protector.Register("alice", "Password", roles: new[] { "Admins" });
Protector.Register("bob", "Password", roles: new[] { "Sales", "TeamLeads" });
Protector.Register("eve", "Password");

Write($"Enter your user name: ");
string? username = ReadLine();
Write($"enter pasword: ");
string? password = ReadLine();

Protector.LogIn(username, password);

if (Thread.CurrentPrincipal == null) {
    WriteLine("log in failed");
    return;
}

IPrincipal p = Thread.CurrentPrincipal;

WriteLine($"IsAuthenticated: {p.Identity?.IsAuthenticated}");
WriteLine($"AuthenticationType: {p.Identity?.AuthenticationType}");
WriteLine($"Name: {p.Identity?.Name}");
WriteLine($"IsInRole(\"Admins\"): {p.IsInRole("Admins")}");
WriteLine($"IsInRole(\"Sales\"): {p.IsInRole("Sales")}");

if (p is ClaimsPrincipal) {
    WriteLine($"{p.Identity?.Name} has claims: ");
    IEnumerable<Claim>? claims = (p as ClaimsPrincipal)?.Claims;

    if (claims is not null) {
        foreach (Claim claim in claims) {
            WriteLine($"{claim.Type}: {claim.Value}");
        }
    }
}

try {
    SecureFeature();
} catch (Exception e) {
    WriteLine($"{e.GetType()}: {e.Message}");
}

static void SecureFeature() {
    if (Thread.CurrentPrincipal == null) {
        throw new SecurityException("a user must be log in to access");
    }
    if (!Thread.CurrentPrincipal.IsInRole("Admins")) {
        throw new SecurityException("user is not Admins role");
    }
    WriteLine("user has access");
}