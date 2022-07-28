using Saltant.Matchmaking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Saltant.Matchmaking.Models
{
    public class ThunderdomeTeam : ITeam
    {
        readonly string playerName;
        readonly PlayerDeck playerDeck;
        readonly Rank rank;
        readonly DateTime startDateTime;
        public ThunderdomeTeam(string playerName, string data, Rank rank = Rank.Training)
        {
            this.playerName = playerName;
            this.rank = rank;

            playerDeck = JsonConvert.DeserializeObject<PlayerDeck>(data);
            startDateTime = DateTime.Now;
        }
        Team.Owner owner;
        internal Team.Owner Owner { get => owner; set => owner = value; }
        internal bool IsElapsed => (startDateTime + TimeSpan.FromSeconds(90) - DateTime.Now).TotalSeconds <= 0;
        public Team.Owner GetOwner => owner;
        public string GetPlayerName { get => playerName; }
        public PlayerDeck GetPlayerDeck => playerDeck;
        public Rank GetRank => rank;

    }
}
