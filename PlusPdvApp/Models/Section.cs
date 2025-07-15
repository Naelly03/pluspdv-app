using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Packaging;

namespace PlusPdvApp.Models;

public class Subsection
{
    [JsonProperty("id")]
    public int Id { get; set;}

    [JsonProperty("name")]
    public string Name { get; set; }


}

public class Section
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("subsections")]
    public List<Subsection> Subsections { get; set; }
}
