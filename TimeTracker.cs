using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Saltant.Matchmaking
{
    public static class TimeTracker
    {
        static readonly Timer trackerTimer;
        static ConcurrentQueue<ThunderdomeTeam> thunderdomeTrainingTeams;
        static ConcurrentQueue<ThunderdomeTeam> thunderdomeRookieTeams;
        static ConcurrentQueue<ThunderdomeTeam> thunderdomeProTeams;
        static ConcurrentQueue<ThunderdomeTeam> thunderdomeVeteranTeams;
        static readonly object locker = new();
        static readonly List<string> playersToRemove = new();
        static public event Action<ThunderdomeTeam> OnElapsedTeam;
        static bool isUpdateRunning;
        static TimeTracker()
        {
            if(trackerTimer == null)
            {
                trackerTimer = new Timer(Update, null, 0, 1000);
            }
        }

        static void Update(object state)
        {
            if (isUpdateRunning) return;
            isUpdateRunning = true;

            if(thunderdomeTrainingTeams != null)
            {
                foreach (var elapsedTeam in thunderdomeTrainingTeams.Where(x => x.IsElapsed))
                {
                    OnElapsedTeam?.Invoke(elapsedTeam);
                }
                thunderdomeTrainingTeams.Update(playersToRemove);
            }

            if(thunderdomeRookieTeams != null)
            {
                foreach (var elapsedTeam in thunderdomeRookieTeams.Where(x => x.IsElapsed))
                {
                    OnElapsedTeam?.Invoke(elapsedTeam);
                }
                thunderdomeRookieTeams.Update(playersToRemove);
            }

            if(thunderdomeProTeams != null)
            {
                foreach (var elapsedTeam in thunderdomeProTeams.Where(x => x.IsElapsed))
                {
                    OnElapsedTeam?.Invoke(elapsedTeam);
                }
                thunderdomeProTeams.Update(playersToRemove);
            }

            if(thunderdomeVeteranTeams != null)
            {
                foreach (var elapsedTeam in thunderdomeVeteranTeams.Where(x => x.IsElapsed))
                {
                    OnElapsedTeam?.Invoke(elapsedTeam);
                }
                thunderdomeVeteranTeams.Update(playersToRemove);
            }

            playersToRemove.Clear();
            isUpdateRunning = false;
        }

        internal static void UpdateThunderdomeTeamsTrack(ConcurrentQueue<ThunderdomeTeam> teams, Rank rank)
        {
            switch (rank)
            {
                case Rank.Training:
                    thunderdomeTrainingTeams = teams;
                    break;
                case Rank.Rookie:
                    thunderdomeRookieTeams = teams;
                    break;
                case Rank.Professional:
                    thunderdomeProTeams = teams;
                    break;
                case Rank.Veteran:
                    thunderdomeVeteranTeams = teams;
                    break;
            }
        }

        internal static void Remove(string playerName)
        {
            lock(locker)
            if (!playersToRemove.Contains(playerName)) playersToRemove.Add(playerName);
        }
    }
}
