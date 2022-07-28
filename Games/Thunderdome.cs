using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;

namespace Saltant.Matchmaking.Games
{
    internal class Thunderdome : GameBase
    {
        readonly ConcurrentBag<ThunderdomeTeam> thunderdomeTeams = new();
        readonly ConcurrentQueue<ThunderdomeTeam> trainingTeams = new();
        readonly ConcurrentQueue<ThunderdomeTeam> rookieTeams = new();
        readonly ConcurrentQueue<ThunderdomeTeam> proTeams = new();
        readonly ConcurrentQueue<ThunderdomeTeam> veteranTeams = new();
        readonly Matchmaker.Events events;
        internal Thunderdome(Matchmaker matchmaker)
        {
            events = matchmaker.GetEvents;
            events.CreateTeam += OnCreateTeam;
            events.RemoveMatch += OnRemoveMatch;
            events.Disconnect += TimeTracker.Remove;
            TimeTracker.OnElapsedTeam += OnElapsedTeam;
        }

        public Matchmaker.Events GetEvents => events;
        public ConcurrentBag<ThunderdomeTeam> GetThunderdomeTeams => thunderdomeTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetTrainingTeams => trainingTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetRookieTeams => rookieTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetProTeams => proTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetVeteranTeams => veteranTeams;

        void OnCreateTeam(Matchmaker.GameType gameType, string playerName, string data, Rank rank)
        {
            if (!IsPlayerExistInMatchmaking(playerName))
            {
                ThunderdomeTeam team = (ThunderdomeTeam)CreateTeam(Matchmaker.GameType.Thunderdome, playerName, data, rank);
                thunderdomeTeams.Add(team);
                TryCreateMatch(thunderdomeTeams, this, rank);
            }
            else events.InvokeEvent(gameType, Matchmaker.Events.EventType.CreateTeamError, $"Player {playerName} already exist in matchmaking queue.", playerName);
        }

        void OnElapsedTeam(ThunderdomeTeam team) => events.InvokeEvent(Matchmaker.GameType.Thunderdome, Matchmaker.Events.EventType.ElapsedTeam, team.GetPlayerName);

        bool IsPlayerExistInMatchmaking(string playerName)
        {
            bool isExist = false;

            if (thunderdomeTeams.Where(x => x.GetPlayerName == playerName).Any())
                isExist = true;
            else if (trainingTeams.Where(x => x.GetPlayerName == playerName).Any())
                isExist = true;
            else if (rookieTeams.Where(x => x.GetPlayerName == playerName).Any())
                isExist = true;
            else if (proTeams.Where(x => x.GetPlayerName == playerName).Any())
                isExist = true;
            else if (veteranTeams.Where(x => x.GetPlayerName == playerName).Any())
                isExist = true;

            return isExist;
        }

        void OnRemoveMatch(IGameMatch gameMatch)
        {
            RemoveGameMatch(gameMatch);
        }
    }
}
