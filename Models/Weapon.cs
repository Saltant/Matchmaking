using Newtonsoft.Json;
namespace Saltant.Matchmaking.Models
{
    public class Weapon
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("attack")] public int Attack { get; set; }
        [JsonProperty("defense")] public int Defence { get; set; }
        [JsonProperty("class")] public string Class { get; set; }
        [JsonProperty("img")] public string Img { get; set; }
    }
}
