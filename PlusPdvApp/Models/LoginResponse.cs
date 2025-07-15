using Newtonsoft.Json;

namespace PlusPdvApp.Models;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("expiration")]
    public string Expiration { get; set; }
}
