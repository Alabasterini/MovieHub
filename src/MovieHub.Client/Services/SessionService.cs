namespace MovieHub.Client.Services;

public class SessionService
{
    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public bool IsLoggedIn => Token != null;
    public bool IsAdmin => Role == "Admin";

    public void SetSession(string token, string username, string role)
    {
        Token = token;
        Username = username;
        Role = role;
    }

    public void Clear()
    {
        Token = null;
        Username = null;
        Role = null;
    }
}