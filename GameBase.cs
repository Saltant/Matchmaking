using Saltant.Matchmaking.Games;
using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Saltant.Matchmaking
{
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
