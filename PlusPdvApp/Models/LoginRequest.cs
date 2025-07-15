using Newtonsoft.Json;

namespace PlusPdvApp.Models;

public class LoginRequest
{
    [JsonProperty("app_id")]  
    public string App_id { get; set; }

    [JsonProperty("store_id")] 
    public string Store_id { get; set; } 

    [JsonProperty("login")] 
    public string Login { get; set; } 

    [JsonProperty("password")] 
    public string Password { get; set; }
}
