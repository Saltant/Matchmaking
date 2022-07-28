using Saltant.Matchmaking.Games;
using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Saltant.Matchmaking
{
    public static class ConcurrentQeueueExtensions
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

    abstract public class GameBase
    {
        readonly List<IGameMatch> gameMatches = new();
        readonly object locker = new();
        ConcurrentQueue<ThunderdomeTeam> thunderdomeTeams;
        public int GetRunningMatchesCount => gameMatches.Count;
        protected ITeam CreateTeam(Matchmaker.GameType gameType, string playerName, string data, Rank rank = Rank.Training)
        {
            Team team = new(gameType, playerName, data, rank);
            return team.Initialize();
        }

        protected void AddGameMatch(IGameMatch gameMatch)
        {
            lock(locker)
                gameMatches.Add(gameMatch);
        }

        protected void RemoveGameMatch(IGameMatch gameMatch)
        {
            lock(locker)
                gameMatches.Remove(gameMatch);
        }

        protected void TryCreateMatch<ITeam>(ConcurrentBag<ITeam> teams, GameBase game, Rank rank = Rank.Training)
        {
            if(game is Thunderdome thunderdome)
            {
                if(teams.TryTake(out ITeam team))
                {
                    thunderdomeTeams = null;
                    switch (rank)
                    {
                        case Rank.Training:
                            thunderdome.GetTrainingTeams.Enqueue(team as ThunderdomeTeam);
                            thunderdomeTeams = thunderdome.GetTrainingTeams.TrackTeamCooldown(rank);
                        break;
                        case Rank.Rookie:
                            thunderdome.GetRookieTeams.Enqueue(team as ThunderdomeTeam);
                            thunderdomeTeams = thunderdome.GetRookieTeams.TrackTeamCooldown(rank);
                            break;
                        case Rank.Professional:
                            thunderdome.GetProTeams.Enqueue(team as ThunderdomeTeam);
                            thunderdomeTeams = thunderdome.GetProTeams.TrackTeamCooldown(rank);
                            break;
                        case Rank.Veteran:
                            thunderdome.GetVeteranTeams.Enqueue(team as ThunderdomeTeam);
                            thunderdomeTeams = thunderdome.GetVeteranTeams.TrackTeamCooldown(rank);
                            break;
                    }
                    CreateThunderdomeMatch(thunderdome, thunderdomeTeams);
                }
            }
        }

        void CreateThunderdomeMatch(Thunderdome thunderdome, ConcurrentQueue<ThunderdomeTeam> teams)
        {
            if (teams.Count < 2) return;

            if(teams.TryDequeue(out ThunderdomeTeam team1) && teams.TryDequeue(out ThunderdomeTeam team2))
            {
                team1.Owner = Team.Owner.Player1;
                team2.Owner = Team.Owner.Player2;

                ThunderdomeGameMatch gameMatch = new(team1.GetRank == team2.GetRank && team1.GetPlayerDeck.PlayerMinions.Count == 5 && team2.GetPlayerDeck.PlayerMinions.Count == 5 ? team1.GetRank : Rank.Training);
                gameMatch.AddTeam(team1);
                gameMatch.AddTeam(team2);
                if (gameMatch.IsTeamFormed)
                {
                    if(!gameMatches.Where(x => x.GetMatchId.Equals(gameMatch.GetMatchId)).Any())
                    {
                        AddGameMatch(gameMatch);
                        thunderdome.GetEvents.InvokeEvent(Matchmaker.GameType.Thunderdome, Matchmaker.Events.EventType.MatchCreated, gameMatch);
                    }
                    else
                    {
                        gameMatch.Destroy();
                        team1.Owner = Team.Owner.None;
                        team2.Owner = Team.Owner.None;
                        teams.Enqueue(team1);
                        teams.Enqueue(team2);
                    }
                }
            }
        }
    }
}
