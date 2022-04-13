using System.Collections.Generic;

namespace Saltant.Matchmaking.Models
{
    public class PlayerDeck
    {
        public Weapon PlayerWeapon { get; set; } = new();
        public Avatar PlayerAvatar { get; set; } = new();
        public List<Minion> PlayerMinions { get; set; } = new();
    }
}
