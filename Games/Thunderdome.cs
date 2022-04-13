using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

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
        }

        public Matchmaker.Events GetEvents => events;
        public ConcurrentBag<ThunderdomeTeam> GetThunderdomeTeams => thunderdomeTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetTrainingTeams => trainingTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetRookieTeams => rookieTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetProTeams => proTeams;
        public ConcurrentQueue<ThunderdomeTeam> GetVeteranTeams => veteranTeams;

        void OnCreateTeam(Matchmaker.GameType gameType, string playerName, string data, Rank rank)
        {
            ThunderdomeTeam team = (ThunderdomeTeam)CreateTeam(Matchmaker.GameType.Thunderdome, playerName, data, rank);
            thunderdomeTeams.Add(team);
            TryCreateMatch(thunderdomeTeams, this, rank);
        }

        void OnRemoveMatch(IGameMatch gameMatch)
        {
            RemoveGameMatch(gameMatch);
        }
    }
}
