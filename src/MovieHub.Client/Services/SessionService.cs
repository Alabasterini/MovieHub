namespace MovieHub.Client.Services;

public class SessionService
{
    public int UserId { get; private set; }
    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public bool IsLoggedIn => Token != null;
    public bool IsAdmin => Role == "Admin";

    public void SetSession(string token, string username, string role, int userId)
    {
        Token = token;
        Username = username;
        Role = role;
        UserId = userId;
    }

    public void Clear()
    {
        Token = null;
        Username = null;
        Role = null;
    }
}