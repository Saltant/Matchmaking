using Saltant.Matchmaking.Games;
using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saltant.Matchmaking
{
    public partial class Matchmaker
    {
        readonly GameType gameType;
        readonly Events events;
        public Matchmaker(GameType gameType)
        {
            this.gameType = gameType;
            events = new();
            events.MatchCreated += OnMatchCreated;
            InitializeGameType();
        }

        void OnMatchCreated(IGameMatch gameMatch)
        {
            switch (gameType)
            {
                case GameType.Thunderdome:
                    MatchCreated.Invoke((ThunderdomeGameMatch)gameMatch);
                    break;
                case GameType.Team5x5:
                    break;
                case GameType.BattleRoyale:
                    break;
                case GameType.Deathmatch:
                    break;
            }
        }

        GameBase game;
        public GameBase GetGame => game;
        public event Action<string> PlayerJoined;
        public event Action<IGameMatch> MatchCreated;
        public GameType GetGameType => gameType;
        internal Events GetEvents => events;

        void InitializeGameType()
        {
            switch (gameType)
            {
                case GameType.None:
                    throw new Exception("Game Type is not selected.");
                case GameType.Thunderdome:
                    game = new Thunderdome(this);
                    break;
                case GameType.Team5x5:
                    throw new NotImplementedException($"{GameType.Team5x5} is not implemented yet.");
                case GameType.BattleRoyale:
                    throw new NotImplementedException($"{GameType.BattleRoyale} is not implemented yet.");
                case GameType.Deathmatch:
                    throw new NotImplementedException($"{GameType.Deathmatch} is not implemented yet.");
            }
        }

        public enum GameType
        {
            None,
            Thunderdome,
            Team5x5,
            BattleRoyale,
            Deathmatch
        }

        public void Join(string playerName, string data, Rank rank = Rank.Training)
        {
            PlayerJoined.Invoke(playerName);
            events.InvokeEvent(gameType, Events.EventType.CreateTeam, playerName, data, rank);
        }

        public void EndMatch(IGameMatch gameMatch)
        {
            events.InvokeEvent(gameType, Events.EventType.RemoveMatch, gameMatch);
        }
    }
}
