using Newtonsoft.Json;
namespace Saltant.Matchmaking.Models
{
    public class Avatar
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("race")] public string Race { get; set; }
        [JsonProperty("template_id")] public int TemplateId { get; set; }
        [JsonProperty("img")] public string Img { get; set; }
    }
}
