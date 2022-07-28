using Saltant.Matchmaking.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Saltant.Matchmaking
{
    public static class ConcurrentQueueExtensions
    {
        public static ConcurrentQueue<ThunderdomeTeam> TrackTeamCooldown(this ConcurrentQueue<ThunderdomeTeam> teams, Rank rank)
        {
            TimeTracker.UpdateThunderdomeTeamsTrack(teams, rank);

            return teams;
        }
        public static void Update(this ConcurrentQueue<ThunderdomeTeam> teams, List<string> playersToRemove)
        {
            List<ThunderdomeTeam> temp = new();
            foreach (var team in teams.Where(x => !x.IsElapsed && !playersToRemove.Contains(x.GetPlayerName))) temp.Add(team);
            teams.Clear();
            temp.ForEach((team) => teams.Enqueue(team));
            temp.Clear();
            temp = null;
        }
    }
}
