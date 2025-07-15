using System.Security.Policy;
using Newtonsoft.Json;

namespace PlusPdvApp.Models;

public class SupplierManufacturer
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nickname")]

    public string Nickname { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }


}
