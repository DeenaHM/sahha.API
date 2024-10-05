using System.Text.Json.Serialization;

namespace sahha.API.Entities;

public class ProfileTokenResponse
{
    [JsonPropertyName("profileToken")]
    public string ProfileToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public long ExpiresIn { get; set; }

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}