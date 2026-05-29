namespace MovieHub.API.Options;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
}
