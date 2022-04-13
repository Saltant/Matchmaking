using Saltant.Matchmaking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltant.Matchmaking.Models
{
    public class ThunderdomeGameMatch : IGameMatch
    {
        readonly Guid matchId;
        readonly List<ThunderdomeTeam> teams = new();
        readonly Rank rank;

        internal ThunderdomeGameMatch(Rank rank)
        {
            matchId = Guid.NewGuid();
            this.rank = rank;
        }

        internal bool IsTeamFormed => teams.Count == 2;

        internal void AddTeam(ThunderdomeTeam thunderdomeTeam)
        {
            teams.Add(thunderdomeTeam);
        }

        public ThunderdomeTeam GetTeam(Team.Owner owner) => teams.Where(x => x.Owner == owner).FirstOrDefault();

        public Guid GetMatchId => matchId;
        public Rank GetRank => rank;

        internal void Destroy() => teams.Clear();
    }

    public enum Rank
    {
        Training,
        Rookie,
        Professional,
        Veteran
    }
}
