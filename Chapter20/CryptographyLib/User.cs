namespace My.Shared;

public class User {
    public string Name { get; set; }
    public string Salt { get; set; }
    public string SaltHashedPassword { get; set; }
    public string[]? Roles { get; set; }

    public User(string name, string salt, string saltHashedPassword, string[]? roles) {
        Name = name;
        Salt = salt;
        SaltHashedPassword = saltHashedPassword;
        Roles = roles;
    }
    
}