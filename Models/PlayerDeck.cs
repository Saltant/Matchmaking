using System.Collections.Generic;
using Newtonsoft.Json;
namespace Saltant.Matchmaking.Models
{
    public class PlayerDeck
    {
        [JsonProperty("weapon")] public Weapon PlayerWeapon { get; set; } = new();
        [JsonProperty("avatar")] public Avatar PlayerAvatar { get; set; } = new();
        [JsonProperty("minions")] public List<Minion> PlayerMinions { get; set; } = new();
    }
}
