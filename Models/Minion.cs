using Newtonsoft.Json;
namespace Saltant.Matchmaking.Models
{
    public class Minion
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("attack")] public int Attack { get; set; }
        [JsonProperty("defense")] public int Defence { get; set; }
        [JsonProperty("element")] public string Element { get; set; }
        [JsonProperty("race")] public string Race { get; set; }
        [JsonProperty("img")] public string Img { get; set; }
    }
}
